namespace MusicEco.Models.Base;

using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global.AbstractLayers;
using MusicEco.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class ModelWeakEventManger {
    private Dictionary<Type, WeakEventHandler<IntChangeEventArgs>> _eventHandlers = [];
    public void Invoke(object sender, int oldKey, int newKey) {
        Type type = sender.GetType();
        if (_eventHandlers.TryGetValue(type, out WeakEventHandler<IntChangeEventArgs>? handler)) {
            IntChangeEventArgs args = new(oldKey, newKey);
            handler.Invoke(sender, args);
        }
    }
    public void Connect(Type type, Action<object?, IntChangeEventArgs> action) {
        if (_eventHandlers.TryGetValue(type, out WeakEventHandler<IntChangeEventArgs>? handler)) {
            handler.Connect(action);
        }
        else {
            WeakEventHandler<IntChangeEventArgs> newHandler = new();
            newHandler.Connect(action);
            _eventHandlers[type] = newHandler;
        }
    }
    public void Disconnect(Type type, Action<object?, IntChangeEventArgs> action) {
        if (_eventHandlers.TryGetValue(type, out WeakEventHandler<IntChangeEventArgs>? handler)) {
            handler.Disconnect(action);
        }
    }
}
//[JsonDerivedType(typeof(ItemModel), typeDiscriminator: "item")]

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(SongModel), typeDiscriminator: "song")]
[JsonDerivedType(typeof(FileModel), typeDiscriminator: "file")]
[JsonDerivedType(typeof(FolderModel), typeDiscriminator: "folder")]
[JsonDerivedType(typeof(ImageModel), typeDiscriminator: "image")]
[JsonDerivedType(typeof(PlaylistModel), typeDiscriminator: "playlist")]
public abstract class BaseModel {
    #region Static
    public static readonly Dictionary<string, int> MaxRowIds = [];
    private static readonly ModelWeakEventManger _eventManager = new();
    public static void OnModelDataChangedConnect<T>(Action<object?, IntChangeEventArgs> action) where T : BaseModel {
        _eventManager.Connect(typeof(T), action);
    }
    public static void OnModelDataChangedDisconnect<T>(Action<object?, IntChangeEventArgs> action) where T : BaseModel {
        _eventManager.Disconnect(typeof(T), action);
    }
    #endregion
    #region Property
    protected int id = -1;
    [JsonIgnore] public virtual int Id => id;
    [JsonIgnore] public int MaxId {
        get => MaxRowIds.GetValueOrDefault(GetType().Name, 0);
        set => MaxRowIds[GetType().Name] = value;
    }
    #endregion
    #region Base
    public void AssignId() {
        id = MaxId + 1;
    }

    public void SetIdNotSave(int id) {
        this.id = id;
    }
    protected void ChangeKey(int newId) {
        int oldId = Id;
        DataStorage.ChangeKey(GetType().Name, oldId, newId);
        _eventManager.Invoke(this, oldId, newId);
    }
    public virtual void Delete() {
        int oldId = Id;
        DataStorage.Delete(GetType().Name, oldId);
        _eventManager.Invoke(this, oldId, -1);
    }
    public virtual void Save() {
        DataStorage.Save(this, id);
        if (Id > MaxId) MaxId = Id;
        _eventManager.Invoke(this, Id, Id);
    }
    #endregion
    #region Utility
    /// <summary>
    /// Shallow copy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public T Copy<T>() where T : BaseModel {
        return (T)Copy();
    }
    /// <summary>
    /// Shallow copy
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public BaseModel Copy() {
        string jsonStr = JsonSerializer.Serialize(this, Setting.SerializeOption);
        BaseModel? copiedModel = JsonSerializer.Deserialize<BaseModel>(jsonStr, Setting.SerializeOption);
        if (copiedModel != null) {
            copiedModel.SetIdNotSave(id);
            return copiedModel;
        }
        else {
            throw new InvalidDataException($"Failed to copy {GetType().FullName}");
        }
    }
    protected static T? Get<T>(int id) where T : BaseModel {
        string className = typeof(T).Name;
        if (DataStorage.Data.TryGetValue(className, out var classData)) {
            if (classData.TryGetValue(id, out var data)) {
                return (T)data;
            }
        }
        return null;
        //throw new KeyNotFoundException(className + " " + id.ToString());
    }       
    public static List<T> GetAll<T>() where T : BaseModel {
        string className = typeof(T).Name;
        if (DataStorage.Data.TryGetValue(className, out var classData)) {
            return classData.Select(data => (T)data.Value).ToList();
        }
        return [];
        //throw new KeyNotFoundException(className);
    }
    #endregion
}