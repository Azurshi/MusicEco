using MusicEco.Common;
using MusicEco.Global.AbstractLayers;
using MusicEco.Models;
using System.Diagnostics;
using System.Security.Cryptography;

using Uri = Android.Net.Uri;
using Application = Android.App.Application;

namespace MusicEco.Platforms.Android;
public class PlatfformStreamFileAbstraction(Uri uri, Stream stream) : TagLib.File.IFileAbstraction {
    private readonly Uri _uri = uri;
    private readonly Stream _stream = stream;

    public string Name => _uri.ToString()!;
    public Stream ReadStream => _stream;
    public Stream WriteStream => throw new NotImplementedException();
    public void Dispose() {
        _stream?.Dispose();
    }
    public void CloseStream(Stream stream) {
        Dispose();
    }
}
public static partial class AndroidMusicScanner {
    public static void ScanAndPush(Uri inputFolderUri) {
        // Block EventSystem
        Stopwatch sw = new();
        int firstFolderId = PushFirstFolder(inputFolderUri);
        Queue<Uri> folderJob = new();
        Queue<Uri> songJob = new();
        sw.Start();
        // Process folder
        FolderModel? folderModel = FolderModel.Get(firstFolderId);
        if (folderModel == null) {
            throw new Exception($"Failed to push first folder {inputFolderUri}");
        }
        bool isFirst = true;
        folderJob.Enqueue(inputFolderUri);
        while (folderJob.Count > 0) {
            Uri folderUri = folderJob.Dequeue();
            int folderId;
            if (isFirst) {
                isFirst = false;
                folderId = firstFolderId;
            }
            else {
                folderId = PushFolder(folderUri);
            }
            if (folderId != -1) {
                (var childFolderUris, var childFileUris) = UriUtility.GetUriChildren(folderUri);
                foreach (var childFolder in childFolderUris) {
                    folderJob.Enqueue(childFolder);
                }
                foreach (var childFile in childFileUris) {
                    string extension = Global.AbstractLayers.File.GetExtension(UriUtility.GetItemPathFromUri(childFile));
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
            Uri fileUri = songJob.Dequeue();
            PushSong(fileUri);
        }
        sw.Stop();
        Debug.WriteLine($"File scan: {sw.ElapsedMilliseconds} ms");
        // Free cache
        sw.Restart();
        DataStorage.ForceSave();
        // Release block
        GC.Collect();
        // Publish

    }
    private static int PushFirstFolder(Uri inputFolderUri) {
        string folderPath = UriUtility.GetItemPathFromUri(inputFolderUri);

        int lastId = -1;
        FolderModel? existsFolder = FolderModel.GetByPath(inputFolderUri);
        if (!Setting.ScannerOverwrite && existsFolder != null) {
            lastId = existsFolder.Id;
        }

        Uri? parentUri = inputFolderUri;
        List<Uri> createQueue = [];
        while (parentUri != null && parentUri.EncodedPath != "/") {
            FolderModel? model = FolderModel.GetByPath(parentUri);
            if (model == null) {
                createQueue.Add(parentUri);
                parentUri = UriUtility.GetParentUri(parentUri);
            }
            else {
                break;
            }
        }
        createQueue.Reverse();
        if (createQueue.Count == 0) {
            FolderModel? model = FolderModel.GetByPath(inputFolderUri);
            if (model != null) {
                lastId = model.Id;
            }
        }
        foreach (var folderUri in createQueue) {
            Debug.WriteLine(folderUri);
            int folderId = PushFolder(folderUri);
            lastId = folderId;
        }
        return lastId;
    }
    private static int PushFolder(Uri folderUri) {
        string folderPath = UriUtility.GetItemPathFromUri(folderUri);

        int folderId = -1;
        DirectoryInfo directoryInfo = new(folderPath);
        FolderModel? existsFolderModel = FolderModel.GetByPath(folderUri);
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
    private static int PushSong(Uri fileUri) {
        string filePath = UriUtility.GetItemPathFromUri(fileUri);

        int fileId = -1;
        FileModel? existsFileModel = FileModel.GetByPath(fileUri);
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
            SongModel songModel = ReadMetadata(fileUri, out byte[]? imageData);
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
    private static SongModel ReadMetadata(Uri fileUri, out byte[]? imageData) {
        try {
            TagLib.File file = TagLibCreatFile(fileUri);
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
            Debug.WriteLine($"Failed to extract tag of {fileUri}");
            imageData = null;
            return new SongModel();
        }
    }
    private static TagLib.File TagLibCreatFile(Uri uri) {
        try {
            using (var stream = GetStreamFromUri(uri)) {
                if (stream == null) {
                    throw new Exception("Failed to read file");
                }
                var file = TagLib.File.Create(new PlatfformStreamFileAbstraction(uri, stream));
                return file;
            }
        }
        catch {
            throw new NotImplementedException();
        }
    }

    private static Stream? GetStreamFromUri(Uri uri) {
        var contextResolver = Application.Context.ContentResolver;
        return contextResolver!.OpenInputStream(uri);
    }
}
