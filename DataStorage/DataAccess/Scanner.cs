using DataStorage.Models;
using Domain.DataAccess;
using Domain.EventSystem;
using Domain.Models;
#if ANDROID
using MusicEco.Platforms.Android;
#endif
using System.Diagnostics;

namespace DataStorage.DataAccess;
public class Scanner : IScanner {
    public int LogStep = 10;
    internal ScannerDataHelper? helper;
    public async Task ScanAsync(
        System.IProgress<Tuple<int, int>> progress, 
        List<string> inputFolderPaths, 
        List<string> extensions, 
        bool quickScan,
        ItemSource source) 
    {
        helper = new(quickScan, source);
        Domain.Events.ScanStartedEventArgs startArgs = new(progress, inputFolderPaths, extensions, quickScan, source);
        EventSystem.Publish(this, startArgs);
        await Task.Run(() => {
            Scan(helper, progress, inputFolderPaths, extensions);
        });
        Domain.Events.ScanCompletedEventArgs completedArgs = new();
        EventSystem.Publish(this, completedArgs);
    }
#if WINDOWS
    private void Scan(
        ScannerDataHelper helper,
        System.IProgress<Tuple<int, int>> progress, 
        List<string> inputFolderPaths, 
        List<string> extensions) 
    {
        DataException exception = new() {
            Info = "Failed to push first folder",
            Affect = "Music has not been scanned, data may be corrupted",
            Solution = "Please restart application and try again or choose different folder. Delete user data if it's corrupted",
        };
        Stopwatch sw = new();
        sw.Start();
        Queue<string> folderJob = [];
        Queue<string> fileJob = [];
        foreach (string inputFolderPath in inputFolderPaths) {
            long firstFolderId = helper.ScanFirstFolder(inputFolderPath);
            FolderModel? folderModel = helper.GetFolder(firstFolderId) ?? throw exception;
            folderJob.Enqueue(folderModel.Path);
        }
        int firstCount = folderJob.Count;
        while (folderJob.Count > 0) {
            long folderId;
            string folderPath = folderJob.Dequeue();
            if (firstCount > 0) {
                firstCount--;
                folderId = helper.GetFolder(folderPath)!.Id;
            }
            else {
                folderId = helper.ScanFolder(folderPath);
            }
            if (folderId != -1) {
                List<string> childFolderPaths = CustomFile.GetFolders(folderPath);
                foreach (var childFolderPath in childFolderPaths) {
                    folderJob.Enqueue(childFolderPath);
                }
                List<string> childFilePaths = CustomFile.GetFiles(folderPath);
                foreach (var childFilePath in childFilePaths) {
                    if (extensions.Contains(CustomFile.GetExtension(childFilePath))) {
                        fileJob.Enqueue(childFilePath);
                    }
                }
            }
        }
        sw.Stop();
        Debug.WriteLine($"Folder scan: {sw.ElapsedMilliseconds} ms");
        sw.Restart();
        int logCount = 0;
        int totalFileJob = fileJob.Count;
        progress.Report(Tuple.Create(0, totalFileJob));
        while (fileJob.Count > 0) {
            string filePath = fileJob.Dequeue();
            helper.ScanSong(filePath);
            logCount++;
            Debug.WriteLine(filePath);
            if (logCount >= this.LogStep) {
                logCount = 0;
                progress.Report(Tuple.Create(totalFileJob - fileJob.Count, totalFileJob));
            }
        }
        sw.Stop();
        Debug.WriteLine($"File scan: {sw.ElapsedMilliseconds} ms");
    }
#elif ANDROID
    private void Scan(
    ScannerDataHelper helper,
    System.IProgress<Tuple<int, int>> progress,
    List<string> inputFolderPaths,
    List<string> extensions) {
        DataException exception = new() {
            Info = "Failed to push first folder",
            Affect = "Music has not been scanned, data may be corrupted",
            Solution = "Please restart application and try again or choose different folder. Delete user data if it's corrupted",
        };
        Stopwatch sw = new();
        sw.Start();
        Queue<Android.Net.Uri> folderJob = [];
        Queue<string> fileJob = [];
        foreach (string inputFolderPath in inputFolderPaths) {
            long firstFolderId = helper.ScanFirstFolder(inputFolderPath);
            FolderModel? folderModel = helper.GetFolder(firstFolderId) ?? throw exception;
            folderJob.Enqueue(UriUtility.GetUri(folderModel.Path));
        }
        int firstCount = folderJob.Count;
        while (folderJob.Count > 0) {
            long folderId;
            Android.Net.Uri folderUri = folderJob.Dequeue();
            string folderPath = UriUtility.GetItemPathFromUri(folderUri);
            if (firstCount > 0) {
                firstCount--;
                folderId = helper.GetFolder(folderPath)!.Id;
            }
            else {
                folderId = helper.ScanFolder(folderPath);
            }
            if (folderId != -1) {
                (var childFolderUris, var childFileUris) = UriUtility.GetUriChildren(folderUri);
                foreach (var childFolder in childFolderUris) {
                    folderJob.Enqueue(childFolder);
                }
                foreach (var childFile in childFileUris) {
                    string extension = CustomFile.GetExtension(UriUtility.GetItemPathFromUri(childFile));
                    if (extensions.Contains(extension)) {
                        fileJob.Enqueue(childFile.ToString()!);
                    }
                }
            }
        }
        sw.Stop();
        Debug.WriteLine($"Folder scan: {sw.ElapsedMilliseconds} ms");
        sw.Restart();
        int logCount = 0;
        int totalFileJob = fileJob.Count;
        progress.Report(Tuple.Create(0, totalFileJob));
        while (fileJob.Count > 0) {
            string filePath = fileJob.Dequeue();
            helper.ScanSong(filePath);
            logCount++;
            Debug.WriteLine(filePath);
            if (logCount >= this.LogStep) {
                logCount = 0;
                progress.Report(Tuple.Create(totalFileJob - fileJob.Count, totalFileJob));
            }
        }
        sw.Stop();
        Debug.WriteLine($"File scan: {sw.ElapsedMilliseconds} ms");
    }
#endif
    public Dictionary<string, HashSet<long>> CheckDiff() {
        if (helper != null) {
            return helper.CheckDiff();
        }
        else {
            throw new Exception("Scan haven't called");
        }
    }
    public void Rollback() {
        helper = null;
    }
    public void Commit() {
        helper?.Commit();
        helper = null;
        Domain.Events.ScanCommitedEventArgs args = new();
        EventSystem.Publish(this, args);
    }
    public void DeleteAllData() {
        Serialization.Data.Clear();
        GlobalDataStorage.Data.Clear();
        GlobalDataStorage.DataTypes.Clear();
        GlobalDataStorage._changed = true;
        Serialization.ForceSave();
    }
}
