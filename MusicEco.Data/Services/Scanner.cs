using Blake3;
using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Repositories;
using SQLiteORM;
using System.Diagnostics;

namespace MusicEco.Data.Services;
internal partial class Scanner: IScanner {
    private class BufferOverflowException: Exception {

    }
    public bool Running { get; private set; } = false;
    private readonly DatabaseContextAsync _db;
    private readonly IServiceProvider _provider;
    private readonly IScanPathService _scanPathService;

    public event EventHandler<bool>? RunningChanged;

    public Scanner(DatabaseContextAsync databaseContext, IServiceProvider serviceProvider, IScanPathService scanPathService) {
        this._db = databaseContext;
        this._provider = serviceProvider;
        this._scanPathService = scanPathService;
    }
    private static Hash256 ComputeHash(Stream stream, byte[] ioBuffer) {
        using(var harsher = Hasher.New()) {
            int read = 0;
            while((read = stream.Read(ioBuffer, 0, ioBuffer.Length)) > 0) {
                harsher.Update(ioBuffer.AsSpan()[..read]);
            }
            Span<byte> output = stackalloc byte[32];
            harsher.Finalize(output);
            return new(output);
        }
    }
    private static Hash256 ComputeHash(Memory<byte> data) {
        Span<byte> output = stackalloc byte[32];
        Hasher.Hash(data.Span, output);
        return new(output);
    }

    public async Task<bool> ScanAndUpdate(ScanProgress progress, List<string> fileExtensions, int scanWorkers, int processWorkers, TimeSpan updateInterval, object? caller = null) {
        this.Running = true;
        this.RunningChanged?.Invoke(caller, this.Running);
        try {
            var folderPaths = await this._scanPathService.GetPaths();
            HandleFileDto dto;
            TimeSpan scanFilesTime;
            TimeSpan handleFileTime;
            TimeSpan pushTime;
            Stopwatch sw = new();
            using (var db = await _db.GetReader()) {
                var connection = db.Connection;
                var existsFiles = await FileRepository.GetAll(db.Connection);
                Dictionary<string, FileEntry> existsFilesMap = existsFiles.ToDictionary(f => f.Path);
                sw.Start();
                var scanResult = await ScanFiles(existsFiles, folderPaths, fileExtensions.ToHashSet(), scanWorkers, progress.ScanFile, updateInterval);
                sw.Stop();
                scanFilesTime = sw.Elapsed;
                sw.Restart();
                dto = await ProcessFile(this._provider, connection, scanResult, processWorkers, progress.ProcessFile, updateInterval);
                sw.Stop();
                handleFileTime = sw.Elapsed;
            }
            using(var db = await _db.GetWriter()) {
                await db.Connection.BeginTransactionAsync();
                try {
                    sw.Restart();
                    await Task.Run(() => PushChange(db.Connection, dto, progress.PushChange, updateInterval));
                    await Task.Run(() => PostScanCache(db.Connection));
                    await db.Connection.CommitTransactionAsync();
                    sw.Stop();
                    pushTime = sw.Elapsed;
                    Debug.WriteLine($"Scan file time: {scanFilesTime.TotalSeconds} s");
                    Debug.WriteLine($"Handle file time: {handleFileTime.TotalSeconds} s");
                    Debug.WriteLine($"Push time: {pushTime.TotalSeconds} s");
                    return true;
                }
                catch {
                    await db.Connection.RollbackTransactionAsync();
                    return false;
                }
            }
        }
        finally {
            GC.Collect();
            this.Running = false;
            this.RunningChanged?.Invoke(caller, this.Running);
        }
    }
}
