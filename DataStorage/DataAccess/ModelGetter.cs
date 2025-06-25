using DataStorage.Models;
using Domain.DataAccess;
using Domain.Models;

namespace DataStorage.DataAccess;
public class ModelGetter : IModelGetter {
    public IFileModel? File(long id) {
        return FileModel.Get<FileModel>(id);
    }

    public List<IFileModel> FileList() {
        return FileModel.GetAll<FileModel>().Select(s => (IFileModel)s).ToList();
    }

    public IFolderModel? Folder(long id) {
        return FolderModel.Get<FolderModel>(id);
    }
    
    public List<IFolderModel> FolderList() {
        return FolderModel.GetAll<FolderModel>().Select(s => (IFolderModel)s).ToList();
    }

    public IPlaylistModel? Playlist(long id) {
        return PlaylistModel.Get<PlaylistModel>(id);
    }

    public List<IPlaylistModel> PlaylistList() {
        return PlaylistModel.GetAll<PlaylistModel>().Select(s => (IPlaylistModel)s).ToList();
    }

    public ISettingField? SettingField(long id) {
        return SettingFieldModel.Get<SettingFieldModel>(id);
    }

    public List<ISettingField> SettingFieldList() {
        return SettingFieldModel.GetAll<SettingFieldModel>().Select(s => (ISettingField)s).ToList();
    }

    public ISongModel? Song(long id) {
        return SongModel.Get<SongModel>(id);
    }

    public List<ISongModel> SongList() {
        return SongModel.GetAll<SongModel>().Select(s => (ISongModel)s).ToList();
    }
}
