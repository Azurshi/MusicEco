using DataStorage.Models;
using Domain.Models;
#if ANDROID
using MusicEco.Platforms.Android;
#endif
using System.Reflection.Metadata;

namespace DataStorage.DataAccess;
internal class ScannerDataHelper {
    internal readonly Dictionary<long, FileModel> FileData;
    internal readonly Dictionary<long, FolderModel> FolderData;
    internal readonly Dictionary<long, SongModel> SongData;
    internal readonly HashSet<long> ExistingFiles;
    internal readonly HashSet<long> ExistingFolders;
    internal readonly HashSet<long> ExistingSongs;
    internal readonly HashSet<long> FoundFiles;
    internal readonly HashSet<long> FoundFolders;
    internal readonly HashSet<long> FoundSongs;
    private readonly ItemSource source;
    private readonly bool quickScan;
    internal ScannerDataHelper(bool quickScan, ItemSource source) {
        this.quickScan = quickScan;
        this.source = source;
        FileData = [];
        ExistingFiles = [];
        List<FileModel> fileList = BaseModel.GetAll<FileModel>();
        foreach (var item in fileList) {
            FileData[item.Id] = (FileModel)item.Copy();
            ExistingFiles.Add(item.Id);
        }
        FolderData = [];
        ExistingFolders = [];
        List<FolderModel> folderList = BaseModel.GetAll<FolderModel>();
        foreach (var item in folderList) {
            FolderData[item.Id] = (FolderModel)item.Copy();
            ExistingFolders.Add(item.Id);
        }
        SongData = [];
        ExistingSongs = [];
        List<SongModel> songList = BaseModel.GetAll<SongModel>();
        foreach (var item in songList) {
            SongData[item.Id] = (SongModel)item.Copy();
            ExistingSongs.Add(item.Id);
        }
        FoundFiles = [];
        FoundFolders = [];
        FoundSongs = [];
    }
    internal long ScanFirstFolder(string folderPath) {
        long lastFolderId = -1;
        FolderModel? existsFolderModel = GetFolder(folderPath);
        if (existsFolderModel != null) lastFolderId = existsFolderModel.Id;
        string? parentPath = folderPath;
        Stack<string> folderStack = [];
        while (parentPath != null) {
            FolderModel? model = GetFolder(parentPath);
            if (model == null) {
                folderStack.Push(parentPath);
            }
            else {
                FoundFolders.Add(model.Id); // Not break, since we need mark found folders
            }
            parentPath = CustomFile.GetDirectoryName(parentPath);
        }
        if (folderStack.Count == 0) {
            FolderModel? model = GetFolder(folderPath);
            if (model != null) lastFolderId = model.Id;
        }
        while (folderStack.Count > 0) {
            folderPath = folderStack.Pop();
            lastFolderId = ScanFolder(folderPath);
        }
        return lastFolderId;
    }
    internal long ScanFolder(string folderPath) {
        if (folderPath == DefaultValue.Empty) return -1;
        long folderId;
        DirectoryInfo directoryInfo = new(folderPath);
        FolderModel? existsFolderModel = GetFolder(folderPath);
        if (existsFolderModel != null) {
            existsFolderModel.ModifiedTime = directoryInfo.LastWriteTime;
            folderId = existsFolderModel.Id;
        }
        else {
            long parentFolderId = -1;
            FolderModel? parentFolderModel = GetFolder(directoryInfo.Parent?.FullName ?? "root");
            if (parentFolderModel != null) parentFolderId = parentFolderModel.Id;
            FolderModel folderModel = new();
            folderModel.AssignId();
            while (FolderData.ContainsKey(folderModel.Id)) folderModel.AssignId();
            folderModel.Path = directoryInfo.FullName;
            folderModel.Name = directoryInfo.Name;
            folderModel.CreationTime = directoryInfo.CreationTime;
            folderModel.ModifiedTime = directoryInfo.LastWriteTime;
            folderModel.ParentId = parentFolderId;
            folderModel.Source = source;
            folderId = folderModel.Id;
            FolderData.Add(folderId, folderModel);
        }
        FoundFolders.Add(folderId);
        return folderId;
    }
    internal long ScanFile(string filePath, string fileType) {
        long fileId;
        FileModel? existsFileModel = GetFile(filePath);
#if WINDOWS
        FileInfo fileInfo = new(filePath);
#elif ANDROID
        FileInfo fileInfo = new(UriUtility.GetItemPathFromUri(Android.Net.Uri.Parse(filePath)!));
#endif
        if (existsFileModel != null) {
            if (!this.quickScan) {
                existsFileModel.ModifiedTime = fileInfo.LastWriteTime;
                existsFileModel.CreationTime = fileInfo.CreationTime;
                existsFileModel.Size = fileInfo.Length;
            }
            fileId = existsFileModel.Id;
        }
        else {
            FolderModel? parentFolder = GetFolder(fileInfo.DirectoryName ?? "root");
            long parentId = -1;
            if (parentFolder != null) parentId = parentFolder.Id;
            FileModel fileModel = new();
            fileModel.AssignId();
            while (FileData.ContainsKey(fileModel.Id)) fileModel.AssignId();
            fileModel.Type = fileType;
#if WINDOWS
            fileModel.Path = fileInfo.FullName;
#elif ANDROID
            fileModel.Path = filePath;
#endif
            fileModel.Name = fileInfo.Name;
            fileModel.CreationTime = fileInfo.CreationTime;
            fileModel.Size = fileInfo.Length;
            fileModel.Sha256 = fileInfo.Length.ToString();
            fileModel.Extension = fileInfo.Extension.Split('.')[^1];
            fileModel.ParentId = parentId;
            fileModel.Source = source;
            fileId = fileModel.Id;
            FileData.Add(fileId, fileModel);
        }
        FoundFiles.Add(fileId);
        return fileId;
    }
    internal long ScanSong(string filePath) {
        long fileId = ScanFile(filePath, DefaultValue.Audio);
        long songId;
        SongModel? existsSongModel = GetSong(fileId);
        if (existsSongModel != null) {
            if (!this.quickScan) {
                SongModel songModel = ScannerFileHelper.ReadMetaData(filePath);
                songModel.Id = existsSongModel.Id;
                songModel.Favourite = existsSongModel.Favourite;
                songModel.PlayCount = existsSongModel.PlayCount;
                songModel.LastPlayed = existsSongModel.LastPlayed;
                SongData[songModel.Id] = songModel;
            }
            songId = existsSongModel.Id;
        }
        else {
            SongModel songModel = ScannerFileHelper.ReadMetaData(filePath);
            songModel.AssignId();
            while (SongData.ContainsKey(songModel.Id)) songModel.AssignId();
            songModel.FileIds.Add(fileId);
            songModel.Favourite = existsSongModel?.Favourite ?? false;
            songModel.PlayCount = existsSongModel?.PlayCount ?? 0;
            songModel.LastPlayed = existsSongModel?.LastPlayed ?? DateTime.MinValue;
            songId = songModel.Id;
            SongData.Add(songId, songModel);
        }
        FoundSongs.Add(songId);
        return songId;
    }
    private FileModel? GetFile(string filePath) {
        foreach (FileModel file in this.FileData.Values) {
            if (file.Path == filePath) {
                return file;
            }
        }
        return null;
    }
    internal FolderModel? GetFolder(string folderPath) {
        foreach (FolderModel folder in this.FolderData.Values) {
            if (folder.Path == folderPath) {
                return folder;
            }
        }
        return null;
    }
    internal FolderModel? GetFolder(long id) {
        return this.FolderData.GetValueOrDefault(id);
    }
    private SongModel? GetSong(long fileId) {
        foreach (SongModel song in this.SongData.Values) {
            if (song.FileIds.Contains(fileId)) {
                return song;
            }
        }
        return null;
    }
    internal Dictionary<string, HashSet<long>> CheckDiff() {
        HashSet<long> missingFiles = [];
        HashSet<long> missingFolders = [];
        HashSet<long> missingSongs = [];
        HashSet<long> newFiles = [];
        HashSet<long> newFolders = [];
        HashSet<long> newSongs = [];
        foreach (long fileId in this.FileData.Keys) {
            if (!FoundFiles.Contains(fileId)) {
                missingFiles.Add(fileId);
            }
            if (!ExistingSongs.Contains(fileId)) {
                newFiles.Add(fileId);
            }
        }
        foreach (long folderId in this.FolderData.Keys) {
            if (!FoundFolders.Contains(folderId)) {
                missingFolders.Add(folderId);
            }
            if (!ExistingFolders.Contains(folderId)) {
                newFiles.Add(folderId);
            }
        }
        foreach (long songId in this.SongData.Keys) {
            if (!FoundSongs.Contains(songId)) {
                missingSongs.Add(songId);
            }
            if (!ExistingSongs.Contains(songId)) {
                newSongs.Add(songId);
            }
        }
        Dictionary<string, HashSet<long>> result = new() {
            {nameof(missingFiles), missingFiles},
            {nameof(missingFolders), missingFolders},
            {nameof(missingSongs), missingSongs},
            {nameof(newFiles), newFiles},
            {nameof(newFolders), newFolders},
            {nameof(newSongs), newSongs}
        };
        return result;
    }
    internal void Commit() {
        HashSet<long> missingFiles = [];
        HashSet<long> missingFolders = [];
        HashSet<long> missingSongs = [];
        foreach (var file in this.FileData.Values) {
            if (!FoundFiles.Contains(file.Id)) {
                missingFiles.Add(file.Id);
            }
        }
        foreach (var folder in this.FolderData.Values) {
            if (!FoundFolders.Contains(folder.Id)) {
                missingFolders.Add(folder.Id);
            }
        }
        foreach (var song in this.SongData.Values) {
            if (!FoundSongs.Contains(song.Id)) {
                missingSongs.Add(song.Id);
            }
        }
        Serialization.Replace(SongData);
        Serialization.Replace(FileData);
        Serialization.Replace(FolderData);
        FileModel.missingIds = missingFiles;
        FolderModel.missingIds = missingFolders;
    }
}
