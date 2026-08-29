using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Entities;
using SQLiteORM;
using System.Diagnostics;
#if WINDOWS || ANDROID
using MusicEco.Platform;
#endif
namespace MusicEco.Data.Services;

internal partial class Scanner {
    private static async Task<HandleFileDto> ProcessFile(IServiceProvider provider, SQLiteReadConnection connection, ScanFileDto scanFileResult, int nWorkers, IProgress<ProcessFileProgress> progress, TimeSpan updateInterval) {
        var fileHashes = connection.Select<Hash256>(
            "SELECT Hash FROM FileEntity").Select(r => r.Item1).ToHashSet();
        var iconHashes = connection.Select<Hash256>(
            "SELECT Hash FROM IconEntity").Select(r => r.Item1).ToHashSet();
        HandleFileDto result = new();
        List<byte[]> iconBuffers = new(nWorkers);
        List<IconEncoderBuffer> encoderBuffers = new(nWorkers);
        List<IIconEncoder> encoders = new(nWorkers);
        for(int i=0; i<nWorkers; i++) {
            iconBuffers.Add(new byte[Config.ScannerIconBufferInitialSize]);
            encoderBuffers.Add(new());
            var encoder = provider.GetRequiredService<IIconEncoder>();
            encoder.Initialize(1);
            encoders.Add(encoder);
        }
        object resultLock = new();
        int totalCount = scanFileResult.NewFiles.Count + scanFileResult.ContentChangedFiles.Count;
        int completedCount = 0;
        Stopwatch throttleSw = Stopwatch.StartNew();
        TimeSpan lastReport = TimeSpan.Zero;
        void Job(int tIndex, int fileIndex, FileEntity file) {
            var iconBuffer = iconBuffers[tIndex];
            var encoderBuffer = encoderBuffers[tIndex];
            var encoder = encoders[tIndex];
            var fileHash = file.Hash;
            var (metadata, byteVector) = ReadMetadata(file.Path);
            Hash256? iconHash = null;
            // Handle Cover icon
            if (byteVector != null) {
                int iconLength = byteVector.Count;
                while (iconLength > iconBuffer.Length) {
                    iconBuffers[tIndex] = new byte[iconBuffer.Length * 2];
                    iconBuffer = iconBuffers[tIndex];
                    Debug.WriteLine($"Scanner: Icon exceed capacity, allocate new buffer: {iconBuffer.Length}");
                }
                byteVector.CopyTo(iconBuffer, 0);
                var iconMemory = iconBuffer.AsMemory(0, iconLength);
                iconHash = ComputeHash(iconMemory);
                // If icon not exists
                bool exists = true;
                lock (resultLock) {
                    if (!iconHashes.Contains(iconHash.Value)) {
                        exists = false;
                        iconHashes.Add(iconHash.Value);
                    }
                }
                if (!exists) { 
                    var encodeResult = encoder.Encode(
                        iconMemory,
                        Config.SmallIconSize, Config.MediumIconSize, Config.LargeIconSize,
                        encoderBuffer.SmallIconBuffer, encoderBuffer.MediumIconBuffer, encoderBuffer.LargeIconBuffer);
                    encodeResult.ThrowIfEmpty();
#if WINDOWS || ANDROID
                    TempFile smallFile = new();
                    smallFile.Write(encoderBuffer.GetSmallIcon(encodeResult).Span);
                    TempFile mediumFile = new();
                    mediumFile.Write(encoderBuffer.GetMediumIcon(encodeResult).Span);
                    TempFile largeFile = new();
                    largeFile.Write(encoderBuffer.GetLargeIcon(encodeResult).Span);
                    lock(resultLock) {
                        result.Icons.Add(new(iconHash.Value, smallFile, mediumFile, largeFile));
                    }
#else
                    throw new NotImplementedException();
#endif
                }
            }
            // Handle metadata
            var databaseEntryPack = AudioMetadataToDatabaseEntry(fileHash, iconHash, metadata);
            bool shouldReport;
            int localCompletedCount;
            lock (resultLock) {
                result.Audios.Add(new(databaseEntryPack.Item1, databaseEntryPack.Item2));
                completedCount++;
                TimeSpan elapsed = throttleSw.Elapsed;
                localCompletedCount = completedCount;
                shouldReport = localCompletedCount == 1 || localCompletedCount == totalCount || elapsed - lastReport > updateInterval;
                if (shouldReport) {
                    lastReport = elapsed;
                }
            }
            if (shouldReport) {
                // This may or  may not block thread depend on implementation of IProgress
                // Progress use non-blocking report
                progress.Report(new(localCompletedCount, totalCount));
            }
        }
        List<FileEntity> files = [];
        // Handle new files
        foreach (var file in scanFileResult.NewFiles) {
            FileEntity fileEntity = FileEntryToEntity(file);
            result.NewFiles.Add(fileEntity);
            // If FileHash not exist
            if (!fileHashes.Contains(file.Hash)) {
                files.Add(fileEntity);
                fileHashes.Add(file.Hash);
            }
        }
        // Handle changed file
        foreach (var file in scanFileResult.ContentChangedFiles) {
            FileEntity fileEntity = FileEntryToEntity(file);
            result.ContentChangedFiles.Add(fileEntity);
            // If FileHash not exist
            if (!fileHashes.Contains(file.Hash)) {
                files.Add(fileEntity);
                fileHashes.Add(file.Hash);
            }
        }
        result.TimeChangedFiles.AddRange(scanFileResult.TimeChangeFiles);
        int nextFile = 0;
        object lockObj = new();
        Task[] workers = new Task[nWorkers];
        for (int tIndex = 0; tIndex < nWorkers; tIndex++) {
            int workerIndex = tIndex;
            workers[tIndex] = Task.Run(() => {
                while (true) {
                    int fileIndex;
                    // Grab next job when available
                    lock (lockObj) {
                        if (nextFile >= files.Count) {
                            return;
                        }
                        fileIndex = nextFile++;
                    }
                    var file = files[fileIndex];
                    Job(workerIndex, fileIndex, file);
                }
            });
        }
        await Task.WhenAll(workers);
        return result;
    }
}
