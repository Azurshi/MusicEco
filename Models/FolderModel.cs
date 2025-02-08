
using MusicEco.Models.Base;
using System.Text.Json.Serialization;

namespace MusicEco.Models;
public class FolderModel : ItemModel {
    #region Property
    public List<FolderModel> GetChildFolder() {
        List<FolderModel> results = [];
        List<FolderModel> models = BaseModel.GetAll<FolderModel>();
        foreach (var model in models) {
            if (model.ParentId == id) {
                results.Add(model);
            }
        }
        return results;
    }
    public List<FileModel> GetChildFile() {
        List<FileModel> results = [];
        List<FileModel> models = BaseModel.GetAll<FileModel>();
        foreach (var model in models) {
            if (model.ParentId == id) {
                results.Add(model);
            }
        }
        return results;
    }
    #endregion
    #region Utility
    public static FolderModel? Get(int id) {
        return BaseModel.Get<FolderModel>(id);
    }
    public static FolderModel? GetByPath(string path) {
        List<FolderModel> models = BaseModel.GetAll<FolderModel>();
        foreach (var model in models) {
            if (model.Path == path) {
                return model;
            }
        }
        return null;
    }
#if ANDROID
    public static FolderModel? GetByPath(Android.Net.Uri uri) {
        return GetByPath(Platforms.Android.UriUtility.GetItemPathFromUri(uri));
    }
#endif
    #endregion
}