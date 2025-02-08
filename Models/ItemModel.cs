using System.Text.Json.Serialization;

namespace MusicEco.Models;
using MusicEco.Common.Value;
using MusicEco.Global.AbstractLayers;
using MusicEco.Models.Base;

public abstract class ItemModel : BaseModel {
    #region Property
    [JsonInclude] public int? ParentId { get; set; }
    [JsonInclude] public string Path { get; set; } = Null.String;
    [JsonInclude] public string Name { get; set; } = Null.String;
    [JsonInclude] public DateTime CreationTime { get; set; } = Null.DateTime;
    [JsonInclude] public DateTime ModifiedTime { get; set; } = Null.DateTime;
    [JsonInclude] public string UriPath { get; set; } = Null.String;
#if ANDROID
    [JsonIgnore] public Android.Net.Uri? Uri { get; set; }
#endif
    [JsonIgnore] public FolderModel? Parent {
        get {
            int? parentId_ = ParentId;
            if (parentId_ != null) {
                return FolderModel.Get((int)parentId_);
            }
            else {
                return null;
            }
        }
    }
    #endregion
    #region Utility
    public string GetDynamicPath() {
        string path = "";
        ItemModel? item = this;
        string lastItempath = "root";
        while (item.Path != "root") {
            path = File.Join(item.Name, path);
            item = item.Parent;
            if (item == null || lastItempath == item.Path) {
                break;
            }
            lastItempath = item.Path;
        }
        return path;
    }
    #endregion
}