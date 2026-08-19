#if ANDROID
using MusicEco.Core.Data;
using MusicEco.Core.Platforms.Android;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Platforms.Android;
using System.Diagnostics;
using System.Security.Cryptography;
using FileInfo = MusicEco.Data.Platforms.Android.FileInfo;
using Uri = Android.Net.Uri;

namespace MusicEco.Data.Services;

internal partial class Scanner {
    private static FileEntry FileInfoToEntry(FileInfo file, byte[] ioBuffer) {
        Hash256 hash;
        using (var fileStream = UriUtility.OpenFile(file.Uri, ioBuffer.Length, FileAccess.Read)) {
            hash = ComputeHash(fileStream!, ioBuffer);
        }
        string fileName = Path.GetFileNameWithoutExtension(file.Name);
        FileEntry entry = new(file.Path, hash, file.LastWriteTimeUtc, file.Name, fileName, file.Length);
        return entry;
    }
    private static async Task<ScanFileDto> ScanFiles(List<FileEntry> existsFiles, IReadOnlyList<string> paths, HashSet<string> fileExtensions, int nWorkers, IProgress<ScanFileProgress> progress, TimeSpan updateInterval) {
        ReadTicks = 0;
        HashTicks = 0;
        FinalizeTicks = 0;

        ScanFileDto result = new();
        Dictionary<string, FileEntry> existsFilesMap = existsFiles.ToDictionary(f => f.Path);
        Queue<Uri> folderQ = new(paths.Select(UriUtility.GetUri).OfType<Uri>());
        List<FileInfo> files = [];
        while (folderQ.Count > 0) {
            var folderURI = folderQ.Dequeue();
            foreach (var childDirectory in UriUtility.GetFolders(folderURI)) {
                folderQ.Enqueue(childDirectory);
                //Debug.WriteLine(childDirectory.ToString());
            }
            foreach (var file in UriQuery.GetFilesInfo(folderURI)) {
                if (fileExtensions.Contains(Path.GetExtension(file.Name))) {
                    files.Add(file);
                    //Debug.WriteLine(files.Count);
                }
            }
        }
        List<byte[]> ioBuffers = new(nWorkers);
        for (int i = 0; i < nWorkers; i++) {
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
            if (existsFilesMap.TryGetValue(file.Path, out var existFile)) {
                if (file.LastWriteTimeUtc != existFile.ModifiedTime) {
                    var fileEntry = FileInfoToEntry(file, ioBuffer);
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
}
#endif