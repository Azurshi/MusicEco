using Domain.DataAccess;
using Domain.Models;
using System.Text.Json.Serialization;

namespace DataStorage.Models;
public class SongModel : BaseModel, ISongModel {
    [JsonInclude] public string Title { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Album { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string AlbumArtist { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Comment { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Composer { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Performer { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Genre { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Lyric { get; set; } = DefaultValue.Empty;
    [JsonInclude] public int Disc { get; set; } = DefaultValue.Count;
    [JsonInclude] public int DiscCount { get; set; } = DefaultValue.Count;
    [JsonInclude] public int Track { get; set; } = DefaultValue.Count;
    [JsonInclude] public int TrackCount { get; set; } = DefaultValue.Count;
    [JsonInclude] public int Year { get; set; } = DefaultValue.Count;
    [JsonInclude] public int PlayCount { get; set; } = DefaultValue.Count;
    [JsonInclude] public bool Favourite { get; set; } = false;
    [JsonInclude] public DateTime? LastPlayed { get; set; } = null;
    [JsonInclude] public List<long> FileIds { get; set; } = [];

    [JsonIgnore] public IFileModel? File {
        get {
            List<FileModel> files = [];
            foreach(long fileId in FileIds) {
                FileModel? file = FileModel.Get<FileModel>(fileId);
                if (file != null && file.Available) {
                    files.Add(file);
                }
            }
            foreach(var file in files) {
                if (file.Source == ItemSource.Windows || file.Source == ItemSource.Androids) {
                    return file;
                }
            }
            foreach(var file in files) {
                return file;
            }
            return null;
        } 
    }

    public bool Available {
        get {
            return File != null;
        }
    }

    public void AddFile(IFileModel file) {
        if (!FileIds.Contains(file.Id)) {
            FileIds.Add(file.Id);
        }
    }
    public void RemoveFile(IFileModel file) {
        if (FileIds.Contains(file.Id)) {
            FileIds.Remove(file.Id);
        }
    }
    public List<IFileModel> GetFiles() {
        List<IFileModel> files = [];
        foreach (long fileId in FileIds) {
            FileModel? file = FileModel.Get<FileModel>(fileId);
            if (file != null && file.Available) {
                files.Add(file);
            }
        }
        return files;
    }


}