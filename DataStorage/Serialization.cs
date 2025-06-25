using DataStorage.Models;
using Domain.EventSystem;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DataStorage; 
public class Serialization {
    private static readonly Dictionary<string, Dictionary<long, BaseModel>> _data = [];
    internal static Dictionary<string, Dictionary<long, BaseModel>> Data => _data;
    private static readonly Dictionary<string, bool> _classTracker = [];
    private static Dictionary<string, Dictionary<long, string>> FastSerialize() {
        if (Flags.IsLocked()) {
            Debug.WriteLine("Persistance locked !!!");
        }
        Dictionary<string, Dictionary<long, string>> result = [];
        foreach (var kvp in _data) {
            if (_classTracker[kvp.Key]) {
                Dictionary<long, string> classData = [];
#if DEBUG
                if (Config.LogClassSate) {
                    Debug.WriteLine("---Key : " + kvp.Key);
                }
#endif
                foreach (var ikvp in kvp.Value) {
                    string data = JsonSerializer.Serialize(ikvp.Value, Config.SerializeOption);
                    classData.Add(ikvp.Key, data);
                }
                result.Add(kvp.Key, classData);
            }
        }
        return result;
    }
    #region Initialize
    public static void InitializeAndLoad() {
        Debug.WriteLine(Config.DataFolderPath);
        CustomFile.CreateDirectoryIfNotExists(Config.DataFolderPath);
        StartLoad();
    }
    public static async Task SaveTask() {
        while (Flags.Running) {
            if (Flags.AutoSave) {
                StartSave();
            }
            await Task.Delay(Config.AutoSaveDelay);
        }
    }
    #endregion
    #region PublicUse
    public static void Load() {
        StartLoad();
    }
    public static void ForceSave() {
        StartSave();
    }
    #endregion
    #region InternalUse
    internal static void Replace<TBaseModel>(Dictionary<long, TBaseModel> data) where TBaseModel: BaseModel {
        string typeName = typeof(TBaseModel).Name;
        _classTracker[typeName] = true;
        _data[typeName] = data.ToDictionary(kvp => kvp.Key, kvp => (BaseModel)kvp.Value);
    }
    internal static void Save(BaseModel instance, long id) {
        string typeName = instance.GetType().Name;
        _classTracker[typeName] = true;
        BaseModel copiedModel = instance.Copy();
        copiedModel.Id = id;
        if (_data.TryGetValue(typeName, out var data)) {
            data[id] = copiedModel;
        }
        else {
            _data[typeName] = new() { { id, copiedModel } };
        }
        Flags.GlobalChanged = true;
    }
    internal static void Delete(string className, long id) {
        _data.TryGetValue(className, out var classData);
        if (classData != null) {
            classData.Remove(id);
            _classTracker[className] = true;
            return;
        }
        throw new KeyNotFoundException();
    }
    #endregion
    #region Save
    private static void StartSave() {
        var changedData = FastSerialize();
        SaveGlobal();
        foreach (var key in _classTracker.Keys) {
            _classTracker[key] = false;
        }
        Flags.GlobalChanged = false;
        Task.Run(() => SaveAll(changedData));
    }
    private static void SaveAll(Dictionary<string, Dictionary<long, string>> data) {
        while (true) {
            if (!Flags.IsLocked()) {
                lock (Flags.AcquireLock()) {
                    foreach (var kvp in data) {
                        try {
                            SaveClass(kvp.Key, kvp.Value);
                        }
                        catch (Exception e) {
                            Debug.WriteLine($"Failed to save {kvp.Key} data", e);
                        }
                    }
                    break;
                }
            }
            Thread.Sleep(100);
        }
        EventSystem.Publish<DataSavedEventArgs>(null, new ());
    }
    private static void SaveClass(string className, Dictionary<long, string> data) {
        Dictionary<long, JsonNode> nodeData = [];
        foreach (var kvp in data) {
            nodeData.Add(kvp.Key, JsonNode.Parse(kvp.Value)!);
        }
        string filePath = Config.GetSaveFilePath(className);
        if (File.Exists(filePath)) { Utility.RenameFile(filePath, Utility.ConvertFilePathToOld(filePath)); }
        using (FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
            using (StreamWriter writer = new(fileStream)) {
                string jsonData = JsonSerializer.Serialize(nodeData, Config.SerializeOption);
                writer.Write(jsonData);
            }
            Utility.DeleteFile(Utility.ConvertFilePathToOld(filePath));
        }
    }
    #endregion
    #region Load
    private static void StartLoad() {
        LoadGlobal();
        var loadedData = LoadAll();
        _data.Clear();
        foreach (var kvp in loadedData) {
            _data[kvp.Key] = kvp.Value;
            _classTracker[kvp.Key] = false;
        }
        EventSystem.Publish<DataLoadedEventArgs>(null, new());
    }
    private static Dictionary<string, Dictionary<long, BaseModel>> LoadAll() {
        Debug.WriteLine("Loading.....");
        while (true) {
            if (!Flags.IsLocked()) {
                lock (Flags.AcquireLock()) {
                    Dictionary<string, Dictionary<long, BaseModel>> result = [];
                    CustomFile.CreateDirectoryIfNotExists(Config.DataFolderPath);
                    List<string> filePaths = CustomFile.GetFiles(Config.DataFolderPath);
                    foreach (string path in filePaths) {
                        string className = CustomFile.GetFileNameWithoutExtension(path);
                        if (!className.Contains("Old")) {
                            result[className] = LoadClass(className);
                        }
                    }
                    Debug.WriteLine("Loaded!");
                    return result;
                }
            }
            Thread.Sleep(100);
        }
    }
    private static Dictionary<long, BaseModel> LoadClass(string className) {
        string filePath = Config.GetSaveFilePath(className);
        string jsonData = File.ReadAllText(filePath);
        Dictionary<long, BaseModel> result = JsonSerializer.Deserialize<Dictionary<long, BaseModel>>(jsonData, options:Config.SerializeOption) ?? throw new Exception($"Failed to parse {className}");
        foreach (var kvp in result) {
            kvp.Value.Id = kvp.Key;
        }
        return result;
    }
    #endregion
    #region Global
    private static string GetGlobalSaveFilePath() {
        string directory = Path.GetDirectoryName(Config.DataFolderPath) ?? "";
        string globalPath = CustomFile.Join(directory, "global.json");
        return globalPath;
    }
    private static void SaveGlobal() {
        if (Flags.GlobalChanged) {
#if DEBUG
            if (Config.LogGlobalState) {
                Debug.WriteLine("---Key global");
            }
#endif
            Dictionary<string, object?> dataType = [];
            foreach (var kvp in GlobalDataStorage.DataTypes) {
                dataType[kvp.Key] = kvp.Value;
            }

            Dictionary<string, Dictionary<string, object?>> fullData = new(){
                {"data", GlobalDataStorage.Data},
                {"type", dataType }
            };
            string filePath = GetGlobalSaveFilePath();
            if (File.Exists(filePath)) { Utility.RenameFile(filePath, Utility.ConvertFilePathToOld(filePath)); }
            using (FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (StreamWriter writer = new(fileStream)) {
                    string jsonData = JsonSerializer.Serialize(fullData, Config.SerializeOption);
#if DEBUG
                    if (Config.PrintGlobalString) {
                        Debug.WriteLine(jsonData);
                    }
#endif
                    writer.Write(jsonData);
                }
                Utility.DeleteFile(Utility.ConvertFilePathToOld(filePath));
            }
            GlobalDataStorage._changed = false;
        }
    }
    private static void LoadGlobal() {
        string filePath = GetGlobalSaveFilePath();
        if (!File.Exists(filePath)) return;
        string jsonData = File.ReadAllText(filePath);
        if (jsonData == "") return;
        var loadedData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(jsonData, options:Config.SerializeOption) ?? throw new Exception($"Failed to parse global");
        Dictionary<string, JsonElement> data = loadedData.GetValueOrDefault("data", []);
        Dictionary<string, JsonElement> dataType = loadedData.GetValueOrDefault("type", []);
        foreach (var kvp in data) {
            JsonElement value = kvp.Value;
            DataType type = Enum.Parse<DataType>(dataType[kvp.Key].GetString()!);
            object? parsedValue = type switch {
                DataType.Null => null,
                DataType.Int => value.GetInt32(),
                DataType.Float => value.GetSingle(),
                DataType.Bool => value.GetBoolean(),
                DataType.String => value.GetString(),
                DataType.Long => value.GetInt64(),
                DataType.Double => value.GetDouble(),
                DataType.DateTime => value.GetDateTime(),
                DataType.ListInt => value.EnumerateArray().Select(e => e.GetInt32()).ToList(),
                DataType.ListFloat => value.EnumerateArray().Select(e => e.GetSingle()).ToList(),
                DataType.ListString => value.EnumerateArray().Select(e => e.GetString()).ToList(),
                DataType.ListBool => value.EnumerateArray().Select(e => e.GetBoolean()).ToList(),
                DataType.ListLong => value.EnumerateArray().Select(e => e.GetInt64()).ToList(),
                DataType.ListDouble => value.EnumerateArray().Select(e => e.GetDouble()).ToList(),
                _ => throw new NotSupportedException($"Type not supported {type}"),
            };
            GlobalDataStorage.DataTypes[kvp.Key] = type;
            GlobalDataStorage.Set(kvp.Key, parsedValue);
        }
    }
    #endregion
}
