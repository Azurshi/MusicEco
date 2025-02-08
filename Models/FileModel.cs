using System.Text.Json.Serialization;

namespace MusicEco.Models;
using MusicEco.Common.Value;
using MusicEco.Models.Base;

public class FileModel : ItemModel {
    #region Property
    [JsonInclude] public long Size { get; set; } = 0;
    [JsonInclude] public string Type { get; set; } = Null.String;
    [JsonInclude] public string Sha256 { get; set; } = String.Empty;
    [JsonInclude] public string Extension { get; set; } = String.Empty;

    #endregion
    #region Utility
    public static FileModel? Get(int id) {
        return BaseModel.Get<FileModel>(id);
    }
    public SongModel? GetSongWithThisFile() {
        return SongModel.GetAll<SongModel>().Where(e => e.FileId == this.id).FirstOrDefault();
    }
    public static FileModel? GetByPath(string path) {
        List<FileModel> models = BaseModel.GetAll<FileModel>();
        foreach (var model in models) {
            if (model.Path == path) {
                return model;
            }
        }
        return null;
    }
    #endregion
#if ANDROID
    #region Android
    public static FileModel? GetByPath(Android.Net.Uri uri) {
        return GetByPath(MusicEco.Platforms.Android.UriUtility.GetItemPathFromUri(uri));
    }
    #endregion
#endif

}