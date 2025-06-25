using Domain.Models;
using System.Text.Json.Serialization;

namespace DataStorage.Models;
public abstract class ItemModel: BaseModel, IItemModel {
    [JsonInclude] public string Path { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Name { get; set; } = DefaultValue.Empty;
    [JsonInclude] public DateTime CreationTime { get; set; }
    [JsonInclude] public DateTime ModifiedTime { get; set; }
    [JsonInclude] public long ParentId { get; set; } = DefaultValue.Id;
    [JsonInclude] public ItemSource Source { get; set; } = ItemSource.Windows;

    [JsonIgnore] internal bool? availble = null;
    [JsonIgnore] public bool Available {
        get {
            if (availble == null) {
                Refresh();
            }
            return (bool)availble!;
        }
    }
    [JsonIgnore] public IFolderModel? Parent {
        get => Get<FolderModel>(ParentId);
        set {
            ParentId = value?.Id ?? -1;
        }
    }
    [JsonIgnore] public string DynamicPath {
        get {
            IFolderModel? parent = Parent;
            string path = this.Name;
            while (parent != null) {
                path = CustomFile.Join(path, parent.Name);
                parent = parent.Parent;
            }
            return path;
        }
    }

    public abstract void Refresh();
}
