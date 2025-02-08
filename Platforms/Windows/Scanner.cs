using MusicEco.Common;
using MusicEco.Global.AbstractLayers;
using MusicEco.Models;
using System.Diagnostics;
using System.Security.Cryptography;

namespace MusicEco.Platforms.Windows;
public static class WindowMusicScanner {
    public static void ScanAndPush(string inputFolderPath) {
        Common.Value.System.EventSystemBlockPublish = true;
        Stopwatch sw = new();
        int firstFolderId = PushFirstFolder(inputFolderPath);
        Queue<string> folderJob = new();
        Queue<string> songJob = new();
        sw.Start();
        // Process folder
        FolderModel? folderModel = FolderModel.Get(firstFolderId) ?? throw new Exception($"Failed to push first folder {inputFolderPath}");
        bool isFirst = true;
        folderJob.Enqueue(folderModel.Path);
        while (folderJob.Count > 0) {
            string folderPath = folderJob.Dequeue();
            int folderId;
            if (isFirst) {
                isFirst = false;
                folderId = firstFolderId;
            }
            else {
                folderId = PushFolder(folderPath);
            }
            if (folderId != -1) {
                string[] childFolders = System.IO.Directory.GetDirectories(folderPath);
                foreach (string childFolder in childFolders) {
                    folderJob.Enqueue(childFolder);
                }
                string[] childFiles = System.IO.Directory.GetFiles(folderPath);
                foreach (string childFile in childFiles) {
                    string extension = Global.AbstractLayers.File.GetExtension(childFile);
                    if (Setting.SupportedExtension.Contains(extension)) {
                        songJob.Enqueue(childFile);
                    }
                }
            }
        }
        sw.Stop();
        Debug.WriteLine($"Folder scan: {sw.ElapsedMilliseconds} ms");
        sw.Restart();
        while (songJob.Count > 0) {
            string filePath = songJob.Dequeue();
            PushSong(filePath);
        }
        sw.Stop();
        Debug.WriteLine($"File scan: {sw.ElapsedMilliseconds} ms");
        ImageModel.FreeCache();
        sw.Restart();
        DataStorage.ForceSave();
        GC.Collect();
        Common.Value.System.EventSystemBlockPublish = false;

    }
    #region Utility
    /// <summary>
    /// Review this function to same format as other !!!
    /// </summary>
    /// <param name="inputFolderPath"></param>
    /// <returns></returns>
    private static int PushFirstFolder(string inputFolderPath) {
        int lastId = -1;
        FolderModel? existsFolder = FolderModel.GetByPath(inputFolderPath);
        if (!Setting.ScannerOverwrite && existsFolder != null) {
            lastId = existsFolder.Id;
        }

        string? parentPath = inputFolderPath;
        List<string> createQueue = [];
        while (parentPath != null) {
            FolderModel? model = FolderModel.GetByPath(parentPath);
            if (model == null) {
                createQueue.Add(parentPath);
                parentPath = System.IO.Path.GetDirectoryName(parentPath);
            }
            else {
                break;
            }
        }
        createQueue.Reverse();
        if (createQueue.Count == 0) {
            FolderModel? model = FolderModel.GetByPath(inputFolderPath);
            if (model != null) {
                lastId = model.Id;
            }
        }
        foreach (var folderPath in createQueue) {
            Debug.WriteLine(folderPath);
            int folderId = PushFolder(folderPath);
            lastId = folderId;
        }
        return lastId;
    }
    private static int PushFolder(string folderPath) {
        if (folderPath == "") {
            return -1;
        }
        int folderId = -1;
        DirectoryInfo directoryInfo = new(folderPath);
        FolderModel? existsFolderModel = FolderModel.GetByPath(folderPath);
        if (!Setting.ScannerOverwrite && existsFolderModel != null) {
            folderId = existsFolderModel.Id;
        }
        else {
            int parentFolderId = -1;
            FolderModel? parentFolderModel = FolderModel.GetByPath(directoryInfo.Parent?.FullName ?? "root");
            if (parentFolderModel != null) {
                parentFolderId = parentFolderModel.Id;
            }
            FolderModel folderModel = new();
            folderModel.AssignId();
            folderModel.Path = directoryInfo.FullName;
            folderModel.Name = directoryInfo.Name;
            folderModel.CreationTime = directoryInfo.CreationTime;
            folderModel.ModifiedTime = directoryInfo.LastWriteTime;
            folderModel.ParentId = parentFolderId;
            folderModel.Save();
            folderId = folderModel.Id;
        }
        return folderId;
    }
    private static int PushSong(string filePath) {
        int fileId = -1;
        FileModel? existsFileModel = FileModel.GetByPath(filePath);
        if (!Setting.ScannerOverwrite && existsFileModel != null) {
            fileId = existsFileModel.Id;
        }
        else {
            FileInfo fileInfo = new(filePath);
            FolderModel? parentFolder = FolderModel.GetByPath(fileInfo.DirectoryName ?? "root");
            int parentId = -1;
            if (parentFolder != null) {
                parentId = parentFolder.Id;
            }
            FileModel fileModel = new();
            fileModel.AssignId();
            fileModel.Path = fileInfo.FullName;
            fileModel.Name = fileInfo.Name;
            fileModel.CreationTime = fileInfo.CreationTime;
            fileModel.ModifiedTime = fileInfo.LastWriteTime;
            fileModel.Size = fileInfo.Length;
            fileModel.Sha256 = fileInfo.Length.ToString();
            fileModel.Extension = fileInfo.Extension.Split('.')[^1];
            fileModel.ParentId = parentId;
            fileModel.Save();
            fileId = fileModel.Id;
        }
        int songId = -1;
        SongModel? existsSongModel = SongModel.GetByFileId(fileId);
        if (!Setting.ScannerOverwrite && existsSongModel != null) {
            songId = existsSongModel.Id;
        }
        else {
            SongModel songModel = ReadMetadata(filePath, out byte[]? imageData);
            songModel.AssignId();
            int imageId = PushImage(ref imageData);
            songModel.ImageId = imageId;
            songModel.FileId = fileId;
            songModel.Favourite = existsSongModel?.Favourite ?? false;
            songModel.PlayCount = existsSongModel?.PlayCount ?? 0;
            songModel.LastPlayed = existsSongModel?.LastPlayed ?? DateTime.MinValue;
            songModel.Save();
            songId = songModel.Id;
        }
        return songId;
    }
    private static int PushImage(ref byte[]? imageData) {
        if (imageData == null) {
            return 0;
        }
        else {
            int imageId = 0;
            string sha256 = GetSha256(ref imageData);
            ImageModel? existsImageModel = ImageModel.GetBySha256(sha256);
            if (!Setting.ScannerOverwrite && existsImageModel != null) {
                imageId = existsImageModel.Id;
            }
            else {
                ImageModel imageModel = new();
                imageModel.AssignId();
                imageModel.Data = imageData;
                string imageFileName = sha256;
                string imageFilePath = Global.AbstractLayers.File.GetCacheImagePath(imageFileName);
                Global.AbstractLayers.File.SaveImage(ref imageData, imageFilePath);

                int fileId = -1;
                FileModel? existsFileModel = FileModel.GetByPath(imageFilePath);
                if (!Setting.ScannerOverwrite && existsFileModel != null) {
                    fileId = existsFileModel.Id;
                }
                else {
                    FileModel? fileModel = new();
                    fileModel.AssignId();
                    fileModel.Path = imageFilePath;
                    fileModel.Name = imageFileName;
                    fileModel.Extension = ".png";
                    fileModel.CreationTime = DateTime.Now;
                    fileModel.ModifiedTime = DateTime.Now;
                    fileModel.Size = new FileInfo(imageFilePath).Length;
                    fileModel.Sha256 = sha256;
                    fileModel.Save();
                    fileId = fileModel.Id;
                }
                imageModel.FileId = fileId;
                imageModel.Save();
                imageId = imageModel.Id;
            }
            return imageId;
        }
    }
    private static string GetSha256(ref byte[] data) {
        byte[] hash = SHA256.HashData(data);
        string result = BitConverter.ToString(hash, 0, hash.Length);
        return result.Replace("-", "");
    }
    private static SongModel ReadMetadata(string filePath, out byte[]? imageData) {
        try {
            TagLib.File file = TagLib.File.Create(filePath);
            if (file.Tag.Pictures.Length > 0) {
                TagLib.IPicture picture = file.Tag.Pictures[0];
                imageData = picture.Data.Data;
            }
            else {
                imageData = null;
            }
            SongModel song = new() {
                Title = file.Tag.Title,
                Album = file.Tag.Album,
                AlbumArtist = string.Join(", ", file.Tag.AlbumArtists),
                Composer = string.Join(", ", file.Tag.Composers),
                Performer = string.Join(", ", file.Tag.Performers),
                Comment = file.Tag.Comment,
                Disc = (int)file.Tag.Disc,
                DiscCount = (int)file.Tag.DiscCount,
                Genre = string.Join(", ", file.Tag.Genres),
                Track = (int)file.Tag.Track,
                TrackCount = (int)file.Tag.TrackCount,
                Year = (int)file.Tag.Year,
                Lyric = file.Tag.Lyrics
            };
            return song;
        }
        catch {
            Debug.WriteLine($"Failed to extract tag of {filePath}");
            imageData = null;
            return new SongModel();
        }
    }
    #endregion
}