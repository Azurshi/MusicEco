using DataStorage.Models;
using Domain.DataAccess;
using Domain.Models;

namespace DataStorage.DataAccess;
public class ModelQuery : IModelQuery {
    public List<ISongModel> Album(string name, bool extractMatch = false) {
        if (extractMatch) {
            return SongModel.GetAll<SongModel>()
                .Where(s => s.Album == name)
                .Select(s => (ISongModel)s).ToList();
        } else {
            return SongModel.GetAll<SongModel>()
                .Where(s => s.Album.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Select(s => (ISongModel)s).ToList();
        }
    }



    public List<IFileModel> File(string path) {
        if (path == "root") {
            return FileModel.GetAll<FileModel>()
                .Where(s => s.ParentId == -1)
                .Select(s => (IFileModel)s).ToList();
        }
        else {
            return FileModel.GetAll<FileModel>()
                .Where(s => s.Path == path)
                .Select(s => (IFileModel)s).ToList();
        }
    }

    public List<IFolderModel> Folder(string path) {
        if (path == "root") {
            return FolderModel.GetAll<FolderModel>()
                .Where(s => s.ParentId == -1)
                .Select(s => (IFolderModel)s).ToList();
        } else {
            return FolderModel.GetAll<FolderModel>()
                .Where(s => s.Path == path)
                .Select(s => (IFolderModel)s).ToList();
        }

    }

    public List<IPlaylistModel> Playlist(string name) {
        return PlaylistModel.GetAll<PlaylistModel>()
            .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .Select(s => (IPlaylistModel)s).ToList();
    }

    public List<ISongModel> Song(string name) {
        return SongModel.GetAll<SongModel>()
            .Where(s => s.Title.Contains(name, StringComparison.OrdinalIgnoreCase))
            .Select(s => (ISongModel)s).ToList();
    }
    public ISongModel? SongByFileId(long fileId) {
        return SongModel.GetAll<SongModel>()
            .Where(s => s.FileIds.Contains(fileId))
            .Select(s => (ISongModel)s).FirstOrDefault();
    }
    public List<ISongModel> FavouriteSongs() {
        return SongModel.GetAll<SongModel>()
            .Where(s => s.Favourite)
            .OrderBy(s => s.Title)
            .Select(s => (ISongModel)s)
            .ToList();
    }
    public List<ISongModel> SongByPlaycount(int threshold, bool greater = true) {
        if (greater) {
            return SongModel.GetAll<SongModel>()
                .Where(s => s.PlayCount >= threshold)
                .OrderBy(s => s.PlayCount)
                .Select(s => (ISongModel)s)
                .ToList();
        } else {
            return SongModel.GetAll<SongModel>()
                .Where(s => s.PlayCount <= threshold)
                .OrderBy(s => s.PlayCount)
                .Select(s => (ISongModel)s)
                .ToList();
        }
    }
    public List<ISettingField> SettingFieldByName(string name) {
        return SettingFieldModel.GetAll<SettingFieldModel>()
            .Where(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .Select(s => (ISettingField)s).ToList();
    }

    public ISettingField? SettingFieldByUniqueName(string uniqueName) {
        return SettingFieldModel.GetAll<SettingFieldModel>()
            .Where(s => s.UniqueName.Equals(uniqueName))
            .FirstOrDefault();
    }
}
