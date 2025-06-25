using DataStorage.Models;
using Domain.DataAccess;
using Domain.Models;
#if ANDROID
using MusicEco.Platforms.Android;
#endif
using System.Diagnostics;

namespace DataStorage.DataAccess;
public class DataGetter : IDataGetter {
    private static readonly Dictionary<string, ImageSource> _cachedImages = [];
    private static readonly ImageSource DefaultImageSource = ImageSource.FromFile("default_image.png");
    private static readonly ImageSource MissingImageSource = ImageSource.FromFile("missing_image.png");
    private static string GetFileExtension(string filePath) {
        return System.IO.Path.GetExtension(filePath);
    }
    public List<string> AlbumNames() {
        return SongModel.GetAll<SongModel>()
            .Select(s => s.Album).Distinct().ToList();
    }
    private static ImageSource? ReadImage(string filePath) {
        try {
            string fileExtension = GetFileExtension(filePath);
            if (Config.AudioFileExtensions.Contains(fileExtension)) {
#if WINDOWS
                TagLib.File file = TagLib.File.Create(filePath, TagLib.ReadStyle.PictureLazy);
#elif ANDROID
                TagLib.File file = ScannerFileHelper.TagLibCreatFile(Android.Net.Uri.Parse(filePath)!);
#endif
                if (file.Tag.Pictures.Length > 0) {
                TagLib.IPicture picture = file.Tag.Pictures[0];
                    ImageSource image = ImageSource.FromStream(() => new MemoryStream(picture.Data.Data));
                    return image;
                }
                else {
                    return null;
                }
            }
            else if (Config.ImageFileExtensions.Contains(fileExtension)) {
                return ImageSource.FromFile(filePath);
            }
            else {
                Debug.WriteLine($"Failed to read image: {filePath}");
                return null;
            }
        }
        catch {
            return null;
        }
    }
    public ImageSource Image(string filePath) {
        ImageSource? cachedImage = _cachedImages.GetValueOrDefault(filePath);
        if (cachedImage != null) {
            return cachedImage;
        }
        else {
            ImageSource? newSource = ReadImage(filePath);
            if (newSource != null) {
                _cachedImages.Add(filePath, newSource);
                return newSource;
            }
            else {
                return DefaultImageSource;
            }
        }
    }
    public ImageSource Image(ISongModel song) {
        IFileModel? fileModel = song.File;
        if (fileModel != null && fileModel.Available) {
            return Image(fileModel.Path);
        } else {
            return MissingImageSource;
        }
    }
    public ImageSource Image(IFileModel file) {
        if (file.Available) {
            string filePath = file.Path;
            return Image(filePath);
        }
        else {
            return MissingImageSource;
        }
    }
    public ImageSource Icon(ISongModel song) {
        return Image(song);
    }
    public ImageSource Icon(IFileModel file) {
        return Image(file);
    }
    public ImageSource Icon(string filePath) {
        return Icon(filePath);
    }
}
