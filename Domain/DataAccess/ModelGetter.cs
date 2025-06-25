using Domain.Models;

namespace Domain.DataAccess;
public interface IModelGetter {
    public ISongModel? Song(long id);
    public List<ISongModel> SongList();
    public IFileModel? File(long id);
    public List<IFileModel> FileList();
    public IFolderModel? Folder(long id);
    public List<IFolderModel> FolderList();
    public IPlaylistModel? Playlist(long id);
    public List<IPlaylistModel> PlaylistList();
    public ISettingField? SettingField(long id);
    public List<ISettingField> SettingFieldList();
}