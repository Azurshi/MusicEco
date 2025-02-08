using System.Text.Json.Serialization;

namespace MusicEco.Models;
using MusicEco.Common.Value;
using MusicEco.Models.Base;

public class PlaylistModel : BaseModel {
    #region Property
    [JsonInclude] public string Name { get; set; } = Null.String;
    [JsonInclude] public string Type { get; set; } = Data.Playlist_PlaylistType;
    [JsonInclude] public List<int> SongIds { get; set; } = [];
    [JsonInclude] public DateTime TimeStamp { get; set; }
    [JsonInclude] public int CurrentSongId { get; set; }
    #endregion
    public int GetNextId() {
        if (SongIds.Count == 0) return -1;
        int index = SongIds.IndexOf(CurrentSongId);
        if (index == -1) {
            return SongIds[0];
        } else if (index == SongIds.Count - 1) {
            return SongIds[0];
        } else {
            return SongIds[index + 1];
        }
    }
    public int GetPreviousId() {
        if (SongIds.Count == 0) return -1;
        int index = SongIds.IndexOf(CurrentSongId);
        if (index == -1) {
            return SongIds[^1];
        }
        else if (index == 0) {
            return SongIds[^1];
        }
        else {
            return SongIds[index - 1];
        }
    }

    #region Utility
    public static PlaylistModel? Get(int id) {
        return BaseModel.Get<PlaylistModel>(id);
    }
    public static PlaylistModel? GetByName(string name) {
        List<PlaylistModel> models = BaseModel.GetAll<PlaylistModel>();
        foreach (var model in models) {
            if (model.Name == name) {
                return model;
            }
        }
        return null;
    }
    #endregion
}