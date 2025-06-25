using Domain.Models;
using System.Text.Json.Serialization;

namespace DataStorage.Models;
public class PlaylistModel : BaseModel, IPlaylistModel {
    [JsonInclude] public string Name { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Type { get; set; } = DefaultValue.Playlist;
    [JsonInclude] public DateTime? LastPlayed { get; set; }
    [JsonInclude] public long CurrentSongId { get; set; } = DefaultValue.Id;
    [JsonInclude] public List<long> SongIds { get; set; } = [];
    [JsonInclude] public int Order { get; set; } = int.MaxValue;

    [JsonIgnore]
    public IReadOnlyList<ISongModel> Songs {
        get {
            List<SongModel> songs = GetAll<SongModel>();
            List<ISongModel> result = [];
            foreach (var id in SongIds) {
                SongModel? target = songs.Where(s => s.Id == id).FirstOrDefault();
                if (target != null) {
                    result.Add(target);
                }
            }
            return result;
        }
    }
    [JsonIgnore]
    public IReadOnlyList<ISongModel> AvailableSongs {
        get {
            List<SongModel> songs = GetAll<SongModel>();
            List<ISongModel> result = [];
            foreach (var id in SongIds) {
                SongModel? target = songs.Where(s => s.Id == id).FirstOrDefault();
                if (target != null && target.Available) {
                    result.Add(target);
                }
            }
            return result;
        }
    }
    [JsonIgnore] public ISongModel? Current {
        get => AvailableSongs.Where(s => s.Id == CurrentSongId).FirstOrDefault();
        set => CurrentSongId = value?.Id ?? -1;
    }
    [JsonIgnore] public ISongModel? NextSong {
        get {
            List<ISongModel> songs = [.. AvailableSongs];
            if (songs.Count == 0) {
                return null;
            }
            ISongModel? current = Current;
            if (current == null) {
                return FirstSong;
            }
            else {
                int index = -1;
                for (int i = 0; i < songs.Count; i++) {
                    if (songs[i].Id == current.Id) {
                        index = i;
                    }
                }
                if (index < songs.Count-1) {
                    return songs[index + 1];
                }
                else {
                    return songs[0];
                }
            }
        }
    }
    [JsonIgnore] public ISongModel? PreviousSong {
        get {
            List<ISongModel> songs = [.. AvailableSongs];
            if (songs.Count == 0) {
                return null;
            }
            ISongModel? current = Current;
            if (current == null) {
                return FirstSong;
            } else {
                int index = -1;
                for(int i=0; i<songs.Count; i++) {
                    if (songs[i].Id == current.Id) {
                        index = i;
                    }
                }
                if (index > 0) {
                    return songs[index - 1];
                }
                else {
                    return songs[^1];
                }
            }
        }
    }

    [JsonIgnore] public ISongModel? FirstSong {
        get {
            List<ISongModel> songs = [.. AvailableSongs];
            if (songs.Count > 0) {
                return songs[0];
            } else {
                return null;
            }
        }
    }

    public void AddSong(ISongModel song) {
        if (!SongIds.Contains(song.Id)) {
            SongIds.Add(song.Id);
        } else {
            DataException e = new() {  
                Info = "Song already exists in " + this.Type
            };
            throw e;
        }
    }

    public void ConsolideOrder(string? type) {
        List<PlaylistModel> all = GetAll<PlaylistModel>().OrderBy(s => s.Order).ToList();
        Dictionary<string, List<PlaylistModel>> map = [];
        foreach (var item in all) {
            if (map.TryGetValue(item.Type, out List<PlaylistModel>? list)) {
                list.Add(item);
            } else {
                map[item.Type] = [item];
            }
        }
        if (type != null) {
            List<PlaylistModel> models = map[type];
            if (type == DefaultValue.Playlist)
            for (int i = 0; i < models.Count; i++) {
                models[i].Order = i;
                models[i].Save();
            }
            else if (type == DefaultValue.Queue) {
                int deleteAmount = Math.Max(0, models.Count - Domain.Config.MaxQueueCount);
                for(int i=0; i<deleteAmount; i++) {
                    models[i].Delete();
                }
                for(int i=deleteAmount;i<models.Count;i++) {
                    models[i].Order = i - deleteAmount;
                    models[i].Save();
                }
            } else {
                throw new Exception($"Type not supported {type}");
            }
        } else {
            foreach(var key in map.Keys) {
                List<PlaylistModel> models = map[key];
                for (int i = 0; i < models.Count; i++) {
                    models[i].Order = i;
                    models[i].Save();
                }
            }
        }
    }

    public void InsertSong(int index, ISongModel song) {
        if (!SongIds.Contains(song.Id)) {
            SongIds.Insert(index, song.Id);
        }
    }

    public bool IsEndOfList() {
        return AvailableSongs.Count > 0 && AvailableSongs[^1].Id == CurrentSongId;
    }

    public void RemoveSong(ISongModel song) {
        SongIds.Remove(song.Id);
    }

    public void Shuffle() {
        List<long> songIds = SongIds;
        songIds.Shuffle();
        SongIds = songIds;
    }
}