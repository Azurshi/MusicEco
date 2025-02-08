using MusicEco.Models.Base;
using System.Diagnostics;
using System.Text.Json.Serialization;
using MusicEco.Global.AbstractLayers;
using MusicEco.Global;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MusicEco.Models;
public class ImageModel : BaseModel {
    #region Property
    [JsonInclude] public int FileId { get; set; }
    [JsonIgnore] public FileModel? File {
        get {
            try {
                return FileModel.Get(FileId);
            }
            catch {
                return null;
            }
        }
    }
    private byte[]? _data;
    private static readonly Dictionary<int, WeakReference<byte[]>> _cacheBytes = [];
    [JsonIgnore] public byte[] Data {
        get {
            if (_data == null) {
                if (_cacheBytes.TryGetValue(this.Id, out WeakReference<byte[]>? bytesRef)
                    && bytesRef.TryGetTarget(out byte[]? bytesData)) {
                    _data = bytesData;
                }
                else {
                    var file = File;
                    if (file != null && Global.AbstractLayers.File.Exists(file.Path)) {
                        _data = Global.AbstractLayers.File.ReadAllBytes(file.Path);
                        _cacheBytes[this.Id] = new WeakReference<byte[]>(_data);
                    }
                    else {
                        Debug.WriteLine($"File not found {file?.Path}. Assign default");
                        _data = Get(0)?.Data ?? throw new NullReferenceException();
                    }
                }
            }
            return _data;
        } set {
            _data = value;
        }
    }
    private static readonly Dictionary<int, WeakReference<ImageSource>> _cachedImage = [];
    [JsonIgnore] public ImageSource Source {
        get {
            if (_cachedImage.TryGetValue(this.Id, out WeakReference<ImageSource>? imageRef)
                && imageRef.TryGetTarget(out ImageSource? image)) {
                return image;
            }
            else {
                ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(Data));
                _cachedImage[this.Id] = new WeakReference<ImageSource>(imageSource);
                return imageSource;
            }
        }
    }
    [JsonIgnore] public ImageSource Icon => Source;
    #endregion
    public static void ImageModelInit() {
        static string GetSha256(byte[] data) {
            using (SHA256 sha256 = SHA256.Create()) {
                byte[] hash = sha256.ComputeHash(data);
                string result = BitConverter.ToString(hash, 0, hash.Length);
                return result.Replace("-", "");
            }
        }
        if (GetAll<ImageModel>().Count == 0) {
            ImageModel imageModel = new();
            imageModel.id = 0;
            var data = ReadImageInResource("raw_default_image.png");
            imageModel.Data = data;
            string path = MusicEco.Global.AbstractLayers.File.GetCacheImagePath("default");
            FileModel? fileModel = FileModel.GetByPath(path);
            if (fileModel == null) {
                fileModel = new();
                fileModel.SetIdNotSave(-2);
                fileModel.Path = path;
                fileModel.Type = "image_png";
                fileModel.Extension = "png";
                fileModel.Name = "default.png";
            }
            fileModel.Sha256 = GetSha256(data);
            imageModel.FileId = fileModel.Id;
            MusicEco.Global.AbstractLayers.File.SaveImage(ref imageModel._data!, fileModel.Path);
            imageModel.Save();
            fileModel.Save();
            Debug.WriteLine("----------- image completed");
        }
        try {
            ImageModel.Get(0);
        }
        catch {
            throw new Exception("Failed to Initialize image");
        }
    }
    private static byte[] ReadImageInResource(string path) {
        Stream stream = FileSystem.OpenAppPackageFileAsync(path).GetAwaiter().GetResult();
        if (stream == null) throw new Exception("File not found " + path);
        using MemoryStream memoryStream = new();
        stream.CopyTo(memoryStream);
        stream.Dispose();
        return memoryStream.ToArray();
    }
    #region Utility
    public static void FreeCache() {
        _cacheBytes.Clear();
        _cachedImage.Clear();
    }
    public static ImageModel? Get(int id) {
        return BaseModel.Get<ImageModel>(id);
    }
    public static ImageModel? GetByFileId(int fileId) {
        List<ImageModel> models = BaseModel.GetAll<ImageModel>();
        foreach (var model in models) {
            if (model.FileId == fileId) {
                return model;
            }
        }
        return null;
    }
    public static ImageModel? GetBySongFileId(int songFileId) {
        SongModel? songModel = SongModel.GetByFileId(songFileId);
        return songModel?.Image;
    }
    public static ImageModel? GetBySha256(string sha256Hash) {
        List<ImageModel> models = BaseModel.GetAll<ImageModel>();
        foreach (var model in models) {
            if (model.File?.Sha256 == sha256Hash) {
                return model;
            }
        }
        return null;
    }
    #endregion
}