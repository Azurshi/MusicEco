using System.Text.Json.Serialization;

namespace MusicEco.Models;
using MusicEco.Common.Value;
using MusicEco.Models.Base;

public class SongModel: BaseModel {
    #region Property
    [JsonInclude] public string Title { get; set; } = Null.String;

    [JsonInclude] public string Album { get; set; } = Null.String;
    [JsonInclude] public string AlbumArtist { get; set; } = Null.String;
    [JsonInclude] public string Comment { get; set; } = Null.String;
    [JsonInclude] public string Composer { get; set; } = Null.String;
    [JsonInclude] public string Performer {  get; set; } = Null.String;
    [JsonInclude] public string Genre { get; set; } = Null.String;
    [JsonInclude] public string Lyric { get; set; } = Null.String;
    [JsonInclude] public int Disc { get; set; }
    [JsonInclude] public int DiscCount { get; set; }
    [JsonInclude] public int Track { get; set; }
    [JsonInclude] public int TrackCount { get; set; }
    [JsonInclude] public int Year { get; set; }
    [JsonInclude] public int PlayCount { get; set; }
    [JsonInclude] public int FileId { get; set; } = -1;
    [JsonInclude] public int ImageId { get; set; } = 0;

    [JsonInclude] public bool Favourite { get; set; } = false;
    [JsonInclude] public DateTime LastPlayed { get; set; } = Null.DateTime;
    [JsonInclude] public string Sha256 { get; set; } = Null.String;

    [JsonIgnore] public FileModel? File => FileModel.Get(FileId);
    [JsonIgnore] public ImageModel? Image => ImageModel.Get(ImageId);
    #endregion
    #region Utility
    public static SongModel? Get(int id) {
        return BaseModel.Get<SongModel>(id);
    }
    public static SongModel? GetByFileId(int fileId) {
        List<SongModel> models = BaseModel.GetAll<SongModel>();
        foreach (var model in models) {
            if (model.FileId == fileId) {
                return model;
            }
        }
        return null;
    }
    public static SongModel? GetByAlbum(string albumName) {
        List<SongModel> models = BaseModel.GetAll<SongModel>();
        foreach (var model in models) {
            if (model.Album == albumName) {
                return model;
            }
        }
        return null;
    }
    public static List<SongModel> SearchByName(string name) {
        List<SongModel> results = [];
        List<SongModel> models = BaseModel.GetAll<SongModel>();
        foreach (var model in models) {
            if (model.Title == null) continue; // !!!!! 
            if (model.Title.Contains(name, StringComparison.OrdinalIgnoreCase)) {
                results.Add(model);
            }
        }
        return results;
    }
    public static List<SongModel> SearchByPlaycount(int threshold) {
        throw new NotImplementedException();
    }
    #endregion
}