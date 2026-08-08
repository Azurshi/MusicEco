using Microsoft.VisualBasic;
using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using System.Diagnostics;

namespace MusicEco.Data.Services;

internal partial class Scanner {
#if WINDOWS
    private static async Task<ScanFileDto> ScanFiles(
        List<FileEntry> existsFiles,
        IReadOnlyList<string> paths,
        HashSet<string> fileExtensions,
        int nWorkers, 
        IProgress<ScanFileProgress> progress,
        TimeSpan updateInterval
        ) {
        ScanFileDto result = new();
        Dictionary<string, FileEntry> existsFilesMap = existsFiles.ToDictionary(f => f.Path);
        Queue<string> folderQ = new(paths);
        List<FileInfo> files = [];
        while (folderQ.Count > 0) {
            string folderPath = folderQ.Dequeue();
            DirectoryInfo directory = new(folderPath);
            foreach (var childDirectory in directory.GetDirectories()) {
                folderQ.Enqueue(childDirectory.FullName);
            }
            foreach (var file in directory.GetFiles()) {
                if (fileExtensions.Contains(file.Extension)) {
                    files.Add(file);
                }
            }
        }
        List<byte[]> ioBuffers = new (nWorkers);
        for(int i=0; i<nWorkers; i++) {
            ioBuffers.Add(new byte[Config.IOBufferSize]);
        }
        object resultLock = new();
        int totalCount = files.Count;
        int completedCount = 0;
        Stopwatch throttleSw = Stopwatch.StartNew();
        TimeSpan lastReport = TimeSpan.Zero;
        object reportLock = new();
        void Job(int tIndex, int fileIndex, FileInfo file) {
            var ioBuffer = ioBuffers[tIndex];
            if (existsFilesMap.TryGetValue(file.FullName, out var existFile)) {
                if (file.LastWriteTimeUtc != existFile.ModifiedTime) {
                    var fileEntry = FileInfoToEntry(file, ioBuffer);
                    if (fileEntry.Hash != existFile.Hash) {
                        lock(resultLock) {
                            result.ContentChangedFiles.Add(fileEntry);
                        }
                    }
                    else {
                        lock(resultLock) {
                            result.TimeChangeFiles.Add(new(fileEntry.Path, fileEntry.ModifiedTime));
                        }
                    }
                }
                // Else skip
            }
            else {
                var fileEntry = FileInfoToEntry(file, ioBuffer);
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
#else
    private static Task<ScanFileDto> ScanFiles(List<FileEntry> existsFiles, IReadOnlyList<string> paths, HashSet<string> fileExtensions, int nWorkers, IProgress<ScanFileProgress> progress, TimeSpan updateInterval) {
        throw new NotImplementedException();
    }
#endif
}
