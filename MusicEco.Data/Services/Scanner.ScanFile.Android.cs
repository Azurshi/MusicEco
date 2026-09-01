#if ANDROID
using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Platforms.Android;
using MusicEco.Platform;
using System.Diagnostics;

using FileInfo = MusicEco.Data.Platforms.Android.FileInfo;
using Uri = Android.Net.Uri;

namespace MusicEco.Data.Services;
internal partial class Scanner {
    private const int ReadFactor = 2;
    private static FileEntry FileInfoToEntry(FileInfo file, byte[] ioBuffer, SemaphoreSlim limit) {
        Hash256 hash;
        //long started = Stopwatch.GetTimestamp();
        // Can read first chunk to increase parallel speed
        // But keep it as now to reduce complexity
        using (var fileStream = UriUtility.OpenFile(file.Uri, ioBuffer.Length, FileAccess.Read)) {
            //Interlocked.Add(ref FinalizeTicks, Stopwatch.GetTimestamp() - started);
            limit.Wait();
            try {
                hash = ComputeHash(fileStream!, ioBuffer);
            }
            finally {
                limit.Release();
            }
        }
        string fileName = Path.GetFileNameWithoutExtension(file.Name);
        FileEntry entry = new(file.Path, hash, file.LastWriteTimeUtc, file.Name, fileName, file.Length);
        return entry;
    }
    private static async Task<ScanFileDto> ScanFiles(List<FileEntry> existsFiles, IReadOnlyList<string> paths, HashSet<string> fileExtensions, int nWorkers, IProgress<ScanFileProgress> progress, TimeSpan updateInterval) {
        ReadTicks = 0;
        HashTicks = 0;
        FinalizeTicks = 0;
        int readFactor = ReadFactor;

        ScanFileDto result = new();
        Dictionary<string, FileEntry> existsFilesMap = existsFiles.ToDictionary(f => f.Path);
        Queue<Uri> folderQ = new(paths.Select(UriUtility.GetUri).OfType<Uri>());
        List<FileInfo> files = [];
        Stopwatch throttleSw = Stopwatch.StartNew();
        TimeSpan lastReport = TimeSpan.Zero;
        void ScanFolderJobs() {
            while (folderQ.Count > 0) {
                var folderURI = folderQ.Dequeue();
                foreach(var childItem in UriQuery.GetItemsInfo(folderURI)) {
                    if (childItem is FolderInfo folderInfo) {
                        folderQ.Enqueue(folderInfo.Uri);
                    }
                    else if (childItem is FileInfo fileInfo) {
                        if (fileExtensions.Contains(Path.GetExtension(fileInfo.Name))) {
                            files.Add(fileInfo);
                        }
                    }
                }
                // Android file / folder access is slow so we should report
                TimeSpan elapsed = throttleSw.Elapsed;
                var shouldReport = elapsed - lastReport > updateInterval;
                if (shouldReport) {
                    lastReport = elapsed;
                }
                if (shouldReport) {
                    progress.Report(new(0, files.Count));
                }
            }
        }
        // Avoid block UI thread
        await Task.Run(ScanFolderJobs);
        List<byte[]> ioBuffers = new(nWorkers);
        List<SemaphoreSlim> processLimits = new(nWorkers);
        for (int i = 0; i < nWorkers; i++) {
            processLimits.Add(new(1));
            ioBuffers.Add(new byte[Config.IOBufferSize]);
        }
        object resultLock = new();
        int totalCount = files.Count;
        int completedCount = 0;
        object reportLock = new();
        void Job(int tIndex, int fileIndex, FileInfo file) {
            int scaledIndex = tIndex / readFactor;
            var ioBuffer = ioBuffers[scaledIndex];
            var limit = processLimits[scaledIndex];
            if (existsFilesMap.TryGetValue(file.Path, out var existFile)) {
                if (file.LastWriteTimeUtc != existFile.ModifiedTime) {
                    var fileEntry = FileInfoToEntry(file, ioBuffer, limit);
                    if (fileEntry.Hash != existFile.Hash) {
                        lock (resultLock) {
                            result.ContentChangedFiles.Add(fileEntry);
                        }
                    }
                    else {
                        lock (resultLock) {
                            result.TimeChangeFiles.Add(new(fileEntry.Path, fileEntry.ModifiedTime));
                        }
                    }
                }
                // Else skip
            }
            else {
                var fileEntry = FileInfoToEntry(file, ioBuffer, limit);
                lock (resultLock) {
                    result.NewFiles.Add(fileEntry);
                }
            }
            bool shouldReport;
            int localCompletedCount;
            lock (reportLock) {
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
        int nextFile = 0;
        object lockObj = new();
        Task[] workers = new Task[nWorkers*readFactor];
        for (int tIndex = 0; tIndex < nWorkers*readFactor; tIndex++) {
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
#endif