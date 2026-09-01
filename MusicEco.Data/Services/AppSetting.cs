using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Repositories;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MusicEco.Data.Services;

internal partial class AppSetting: IAppSetting, IDisposable {
    public event EventHandler<SettingChangedEventArgs>? ItemChanged;
    private readonly JsonSerializerOptions _options;
    private readonly DictionaryRepository _dictRepo;
    private readonly GlobalRepository _globalRepo;
    private readonly Dictionary<string, object?> _settings;
    private bool _disposed = false;
    private readonly Stopwatch _sw;
    public AppSetting(DictionaryRepository dictionaryRepository, GlobalRepository globalRepository) {
        this._dictRepo = dictionaryRepository;
        this._globalRepo = globalRepository;
        this._options = new();
        this.Register(new Hash256JsonConverter());
        this._settings = [];
        this._sw = Stopwatch.StartNew();
        var reader = ServiceRegister.GetReader();
        var rows = reader.Select<string>("SELECT EntryValue FROM DictionaryEntry WHERE EntryKey = ?", nameof(AppSetting)).ToList();
        if (rows.Count > 0) {
            string json = rows[0].Item1;
            var loadedSetting = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) ?? [];
            foreach (var (key, value) in loadedSetting) {
                this._settings[key] = value;
            }
        }
        reader.Dispose(false);
        SaveLoop().FireAndForgetAsync();
    }
    public T Get<T>(T defaultValue, string key) {
        if (this._settings.TryGetValue(key, out var valueObj)) {
            if (valueObj is T value) {
                return value;
            }
            else if (valueObj is JsonElement json) {
                value = JsonSerializer.Deserialize<T>(json, _options) ?? defaultValue;
                this._settings[key] = value;
                return value;
            }
            else {
                if (valueObj != null) {
                    string foundType = valueObj.GetType().ToString();
                    throw new InvalidCastException($"Type mismatch. Required type: {typeof(T)}. Found type: {foundType}");
                }
                else {
                    return (T)valueObj!;
                }
            }
        }
        else {
            return defaultValue;
        }
    }
    public void Set(object? value, string key) {
        this._settings[key] = value;
        ScheduleSave();
    }
    public bool Register(Type type, JsonConverter converter) {
        this._options.Converters.Add(converter);
        return true;
    }
    
    public bool Register<T>(JsonConverter<T> converter) {
        this._options.Converters.Add(converter);
        return true;
    }
    #region Scheduler
    private TimeSpan _requestTime = TimeSpan.Zero;
    private bool _needSave = false;
    private static readonly TimeSpan _saveDelay = TimeSpan.FromMilliseconds(Config.SaveDelayMs);
    private void ScheduleSave() {
        if (!this._needSave) {
            this._needSave = true;
            this._requestTime = this._sw.Elapsed;
        }
    }
    private async Task SaveLoop() {
        while(!_disposed) {
            var elapsed = this._sw.Elapsed;
            if (this._needSave && elapsed - this._requestTime > _saveDelay) {
                this._needSave = false;
                try {
                    await SaveData();
                }
                catch (Exception e) {
                    Debug.WriteLine($"{e} : {e.Message}");
                }
            }
            await Task.Delay(Config.SaveLoopMs);
        }
    }
    private async Task SaveData() {
        string json = JsonSerializer.Serialize(this._settings, _options);
        await _dictRepo.SetValue(nameof(AppSetting), json);
    }
    public void Dispose() {
        this._disposed = true;
    }

    public async Task DeleteAllData() {
        await this._globalRepo.DeleteAllData();
    }
    #endregion
}
