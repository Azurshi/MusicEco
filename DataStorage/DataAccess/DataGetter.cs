using DataStorage.Models;
using Domain.DataAccess;
using Domain.Models;
#if ANDROID
using MusicEco.Platforms.Android;
#endif
using System.Diagnostics;

namespace DataStorage.DataAccess;
public class DataGetter : IDataGetter {
    //private static readonly ImageSource DefaultImageSource = ImageSource.FromFile("default_image.png");
    private static readonly ImageSource MissingImageSource = ImageSource.FromFile("missing_image.png");
    public List<string> AlbumNames() {
        return SongModel.GetAll<SongModel>()
            .Select(s => s.Album).Distinct().ToList();
    }
    public ImageSource Image(string filePath) {
        return ImageManager.ExtractImage(filePath);
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
