using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;
using Domain.Models;

namespace DataStorage.Models;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SongModel), typeDiscriminator: "song")]
[JsonDerivedType(typeof(FileModel), typeDiscriminator: "file")]
[JsonDerivedType(typeof(FolderModel), typeDiscriminator: "folder")]
[JsonDerivedType(typeof(PlaylistModel), typeDiscriminator: "playlist")]
[JsonDerivedType(typeof(SettingFieldModel), typeDiscriminator: "setting_field")]
public abstract class BaseModel: IBaseModel {
    #region Base
    [JsonIgnore] protected long id = -1;
    [JsonInclude]
    public long Id {
        get => id;
        set => this.id = value;
    }
    public void AssignId() {
        Random random = new();
        long id = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000000 + random.Next(0, 1000000);
        Id = id;
    }
    public virtual void Delete() {
        Serialization.Delete(GetType().Name, id);
    }
    public virtual void Save() {
        Serialization.Save(this, id);
    }
    #endregion
    #region Utility
    public T Copy<T>() where T : BaseModel {
        return (T)Copy();
    }
    public BaseModel Copy() {
        string jsonStr = JsonSerializer.Serialize(this, Config.SerializeOption);
        BaseModel? copiedModel = JsonSerializer.Deserialize<BaseModel>(jsonStr, Config.SerializeOption);
        if (copiedModel != null) {
            copiedModel.id = id;
            return copiedModel;
        }
        else {
            throw new InvalidDataException($"Failed to copy {GetType().FullName}");
        }
        throw new NotImplementedException();
    }
    public static T? Get<T>(long id) where T : BaseModel {
        string className = typeof(T).Name;
        if (Serialization.Data.TryGetValue(className, out var classData)) {
            if (classData.TryGetValue(id, out var data)) {
                return (T)data.Copy();
            }
        }
        return null;
    }
    public static List<T> GetAll<T>() where T : BaseModel {
        string className = typeof(T).Name;
        if (Serialization.Data.TryGetValue(className, out var classData)) {
            return classData.Select(data => (T)data.Value.Copy()).ToList();
        }
        return [];
    }
    #endregion
}
