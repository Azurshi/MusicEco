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
    private static readonly ImageSource MissingIconSource = ImageSource.FromFile("missing_icon.png");
    public List<string> AlbumNames() {
        return SongModel.GetAll<SongModel>()
            .Select(s => s.Album).Distinct().ToList();
    }
    public async Task<ImageSource> Image(string filePath) {
        return await ImageManager.ExtractImage(filePath);
    }
    public async Task<ImageSource> Image(ISongModel song) {
        IFileModel? fileModel = song.File;
        if (fileModel != null && fileModel.Available) {
            return await Image(fileModel.Path);
        } else {
            return MissingImageSource;
        }
    }
    public async Task<ImageSource> Image(IFileModel file) {
        if (file.Available) {
            string filePath = file.Path;
            return await Image(filePath);
        }
        else {
            return MissingImageSource;
        }
    }
    public async Task<ImageSource> Icon(ISongModel song) {
        IFileModel? fileModel = song.File;
        if (fileModel != null && fileModel.Available) {
            return await Icon(fileModel.Path);
        }
        else {
            return MissingIconSource;
        }
    }
    public async Task<ImageSource> Icon(IFileModel file) {
        if (file.Available) {
            string filePath = file.Path;
            return await Icon(filePath);
        }
        else {
            return MissingIconSource;
        }
    }
    public async Task<ImageSource> Icon(string filePath) {
        return await ImageManager.ExtractIcon(filePath);
    }
}
