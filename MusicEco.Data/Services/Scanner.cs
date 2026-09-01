using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Repositories;
using SQLiteORM;
using System.Diagnostics;
using System.Security.Cryptography;

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
    private static bool DebugMode = false;
    public async Task<IScanResult> ScanAndUpdate(ScanProgress progress, List<string> fileExtensions, int scanWorkers, int processWorkers, TimeSpan updateInterval, object? caller = null, bool verbose = false) {
        this.Running = true;
        this.RunningChanged?.Invoke(caller, this.Running);
        DebugMode = verbose;
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
                // Can merge Scan and Process to avoid double file open & better resource utilization
                // But keep them separate for easier debug & profile
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
                    if (!DebugMode) {
                        return new SimpleScanResult(true);
                    }
                    else {
#if WINDOWS
                        int factor = 1;
#elif ANDROID
                        int factor = 100;
#endif
#if ANDROID || WINDOWS

                        ValueTuple<TimeSpan, TimeSpan, TimeSpan> extra = (TimeSpan.FromTicks(ReadTicks / factor), TimeSpan.FromTicks(HashTicks / factor), TimeSpan.FromTicks(FinalizeTicks / factor));
                        return new DetailScanResult(scanFilesTime, handleFileTime, pushTime, extra);
#else
                        return new DetailScanResult(scanFilesTime, handleFileTime, pushTime);
#endif
                    }
                }
                catch {
                    await db.Connection.RollbackTransactionAsync();
                    return new SimpleScanResult(true);
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
