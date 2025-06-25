using Domain.Models;

namespace Domain.DataAccess;
public interface IModelQuery {
    public List<ISongModel> Song(string name);
    public List<IPlaylistModel> Playlist(string name);
    public List<IFileModel> File(string path);
    public List<IFolderModel> Folder(string path);
    public List<ISongModel> Album(string name, bool extractMatch = true);
    public ISongModel? SongByFileId(long fileId);
    public List<ISongModel> FavouriteSongs();
    public List<ISongModel> SongByPlaycount(int threshold, bool greater = true);
    public List<ISettingField> SettingFieldByName(string name);
    public ISettingField? SettingFieldByUniqueName(string uniqueName);
}
