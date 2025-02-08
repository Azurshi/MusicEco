using MusicEco.Common;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MusicEco.Global.Attributes;
using MusicEco.Models.Base;

namespace MusicEco.Global.AbstractLayers;
public static class DataStorage {
    public static readonly object PersistanceLock = new();
    public static string DataFolderPath => Setting.DataFolderPath;
    private static readonly Dictionary<string, Dictionary<int, BaseModel>> _data = [];
    public static Dictionary<string, Dictionary<int, BaseModel>> Data => _data;
    private static readonly Dictionary<string, bool> _classTracker = [];
    public static Dictionary<string, Dictionary<int, string>> FastSerialize() {
        if (Util.IsLocked(PersistanceLock)) {
            Debug.WriteLine("Persistance locked !!!");
        }
        Dictionary<string, Dictionary<int, string>> result = [];
        foreach (var kvp in _data) {
            if (_classTracker[kvp.Key]) {
                Dictionary<int, string> classData = [];
                Debug.WriteLine("---Key : " + kvp.Key);
                foreach (var ikvp in kvp.Value) {
                    string data = JsonSerializer.Serialize(ikvp.Value, Setting.SerializeOption);
                    classData.Add(ikvp.Key, data);
                }
                result.Add(kvp.Key, classData);
            }
        }
        return result;
    }
    #region Initialize
    [StaticInitializer(10)]
    public static async Task Initialize() {
        Debug.WriteLine(DataFolderPath);
        File.CreateDirectoryIfNotExist(DataFolderPath);
        StartLoad();
        while (Common.Value.System.AppRunning) {
            Timer_Elapsed();
            await Task.Delay(1000);
        }
    }
    private static void Timer_Elapsed() {
        if (Setting.AutoSave) {
            StartSave();
        }
    }
    #endregion
    #region PublicUse
    public static void Save(BaseModel instance, int id) {
        string typeName = instance.GetType().Name;
        _classTracker[typeName] = true;
        BaseModel copiedModel = instance.Copy();
        copiedModel.SetIdNotSave(id);
        if (_data.TryGetValue(typeName, out var data)) {
            data[id] = copiedModel;
        }
        else {
            _data[typeName] = new() { { id, copiedModel } };
        }
        GlobalData.ChangeTracker = true;
    }
    public static void ChangeKey(string className, int oldId, int newId) {
        _data.TryGetValue(className, out var classData);
        if (classData != null) {
            classData.TryGetValue(oldId, out var data);
            if (data != null) {
                classData.Remove(oldId);
                classData.Add(newId, data);
                _classTracker[className] = true;
                return;
            }
        }
        throw new KeyNotFoundException();
    }
    public static void Delete(string className, int id) {
        _data.TryGetValue(className, out var classData);
        if (classData != null) {
            classData.Remove(id);
            _classTracker[className] = true;
            return;
        }
        throw new KeyNotFoundException();
    }
    public static void Load() {
        StartLoad();
        // Publish event
    }
    public static void ForceSave() {
        StartSave();
    }
    #endregion
    #region Save
    private static void StartSave() {
        var changedData = FastSerialize();
        SaveGlobal();
        foreach (var key in _classTracker.Keys) {
            _classTracker[key] = false;
        }
        GlobalData.ChangeTracker = false;
        Task.Run(() => SaveAll(changedData));
    }
    private static void SaveAll(Dictionary<string, Dictionary<int, string>> data) {
        while (true) {
            if (!Util.IsLocked(PersistanceLock)) {
                lock (PersistanceLock) {
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
        EventSystem.Publish(Common.Events.Signal.System_Data_Saved, null);
    }
    private static void SaveClass(string className, Dictionary<int, string> data) {
        Dictionary<int, JsonNode> nodeData = [];
        foreach (var kvp in data) {
            nodeData.Add(kvp.Key, JsonNode.Parse(kvp.Value)!);
        }
        string filePath = GetSaveFilePath(className);
        if (File.Exists(filePath)) { RenameFile(filePath, ConvertFilePathToOld(filePath)); }
        using (FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
            using (StreamWriter writer = new(fileStream)) {
                string jsonData = JsonSerializer.Serialize(nodeData, Setting.SerializeOption);
                writer.Write(jsonData);
            }
            DeleteFile(ConvertFilePathToOld(filePath));
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
        EventSystem.Publish(Common.Events.Signal.System_Data_Loaded, null);
    }
    private static Dictionary<string, Dictionary<int, BaseModel>> LoadAll() {
        Debug.WriteLine("Loading.....");
        while (true) {
            if (!Util.IsLocked(PersistanceLock)) {
                lock (PersistanceLock) {
                    Dictionary<string, Dictionary<int, BaseModel>> result = [];
                    Global.AbstractLayers.File.CreateDirectoryIfNotExist(DataFolderPath);
                    List<string> filePaths = File.GetFiles(DataFolderPath);
                    foreach (string path in filePaths) {
                        string className = File.GetFileNameWithouExtension(path);
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
    private static Dictionary<int, BaseModel> LoadClass(string className) {
        string filePath = GetSaveFilePath(className);
        string jsonData = File.ReadAllText(filePath);
        Dictionary<int, BaseModel> result = JsonSerializer.Deserialize<Dictionary<int, BaseModel>>(jsonData) ?? throw new Exception($"Failed to parse {className}");
        foreach(var kvp in result) {
            kvp.Value.SetIdNotSave(kvp.Key);
        }
        return result;
    }
    #endregion
    #region Utility
    private static string GetSaveFilePath(string className) {
        return File.Join(DataFolderPath, className+".json");
    }
    private static string ConvertFilePathToOld(string filePath) {
        string directory = Path.GetDirectoryName(filePath) ?? "";
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);
        string newFileName = $"{fileName}Old{extension}";
        string newPath = Path.Combine(directory, newFileName);
        return newPath;
    }
    private static bool RenameFile(string oldPath, string newPath) {
        if (File.Exists(oldPath)) {
            return File.Move(oldPath, newPath);
        }
        else {
            return false;
        }
    }
    private static bool DeleteFile(string filePath) {
        return File.Delete(filePath);
    }
    #endregion
    #region Global
    private static string GetGlobalSaveFilePath() {
        string directory = Path.GetDirectoryName(DataFolderPath) ?? "";
        string globalPath = File.Join(directory, "global.json");
        return globalPath;
    }
    private static void SaveGlobal() {
        if (GlobalData.ChangeTracker) {
            Dictionary<string, object?> dataType = [];
            foreach(var kvp in GlobalData.DataTypes) {
                dataType[kvp.Key] = kvp.Value;
            }
            Dictionary<string, object?> maxRowIds = [];
            foreach (var kvp in BaseModel.MaxRowIds) {
                maxRowIds[kvp.Key] = kvp.Value;
            }

            Dictionary<string, Dictionary<string, object?>> fullData = new(){
                {"data", GlobalData.Data},
                {"type", dataType},
                {"maxRowIds",  maxRowIds}
            };
            string filePath = GetGlobalSaveFilePath();
            if (File.Exists(filePath)) { RenameFile(filePath, ConvertFilePathToOld(filePath)); }
            using (FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (StreamWriter writer = new(fileStream)) {
                    string jsonData = JsonSerializer.Serialize(fullData, Setting.SerializeOption);
                    writer.Write(jsonData);
                }
                DeleteFile(ConvertFilePathToOld(filePath));
            }
        }
        GlobalData.ChangeTracker = false;
    }
    private static void LoadGlobal() {
        string filePath = GetGlobalSaveFilePath();
        if (!File.Exists(filePath)) return;
        string jsonData = File.ReadAllText(filePath);
        if (jsonData == "") return;
        var loadedData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(jsonData) ?? throw new Exception($"Failed to parse global");
        Dictionary<string, JsonElement> data = loadedData.GetValueOrDefault("data", []);
        Dictionary<string, JsonElement> dataType = loadedData.GetValueOrDefault("type", []);
        Dictionary<string, JsonElement> maxRowIds = loadedData.GetValueOrDefault("maxRowIds", []);
        foreach (var kvp in maxRowIds) {
            BaseModel.MaxRowIds[kvp.Key] = kvp.Value.GetInt32();
        }
        foreach (var kvp in data) {
            JsonElement value = kvp.Value;
            int intDataType = dataType[kvp.Key].GetInt32();
            DataType type = (DataType)intDataType;
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
            GlobalData.DataTypes[kvp.Key] = type;
            GlobalData.Set(kvp.Key, parsedValue);
        }
    }
    #endregion
}
