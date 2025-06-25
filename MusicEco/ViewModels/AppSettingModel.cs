using CommunityToolkit.Mvvm.Input;
using Domain.EventSystem;
using Domain.Models;
using MusicEco.ViewModels.Items;
using MusicEco.ViewModels.Settings;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MusicEco.ViewModels;
/// <summary>
/// <see cref="StaticInitializerAttribute"/> priority 6
/// </summary>
public partial class AppSettingModel: PropertyObject, IServiceAccess {
    #region Settings
    private Dictionary<string, BaseSetting> settings = new() {
        {nameof(GridRows), new GridRowsSetting() },
        {nameof(GridColumns), new GridColumnsSetting() },
        {nameof(ListItems), new ListItemsSetting() },
        {nameof(GridPreload), new GridPreloadSetting() },
        {nameof(ListPreload), new ListPreloadSetting() },
        {nameof(MusicPlayerUpdateDelay), new MusicPlayerUpdateDelaySetting() },
        {nameof(Theme), new ThemeSetting() }
    };
    public string Theme {
        get => GetValue<string>();
        set => SetValue(value);
    }
    public SettingFieldModel ThemeModel => Get();
    public int GridRows {
        get => GetValue<int>();
        set => SetValue(value);
    }
    public SettingFieldModel GridRowsModel => Get();
    public int GridColumns {
        get => GetValue<int>();
        set => SetValue(value);
    }
    public SettingFieldModel GridColumnsModel => Get();
    public int ListItems {
        get => GetValue<int>();
        set => SetValue(value);
    }
    public SettingFieldModel ListItemsModel => Get();
    public int GridPreload {
        get => GetValue<int>();
        set => SetValue(value);
    }
    public SettingFieldModel GridPreloadModel => Get();
    public int ListPreload {
        get => GetValue<int>();
        set => SetValue(value);
    }
    public SettingFieldModel ListPreloadModel => Get();
    public int MusicPlayerUpdateDelay {
        get => GetValue<int>();
        set => SetValue(value);
    }
    public SettingFieldModel MusicPlayerUpdateDelayModel => Get();

    #endregion
    public static AppSettingModel? _current;
    public static AppSettingModel Current => _current ?? throw new NullReferenceException();
    public class SettingChangedEventArgs : EventArgs { }
    public AppSettingModel() {
        if (_current != null) throw new Exception("Tried to create singleton");
        _current = this;
        SettingFieldModel Create(string name) {
            return new SettingFieldModel(settings[name].SettingField ?? throw new ArgumentNullException());
        }
        Debug.WriteLine($"~~~~~ Create new instance of {nameof(AppSettingModel)}");
        List<ISettingField> fields = IServiceAccess.ModelGetter.SettingFieldList();
        Dictionary<string, ISettingField> fieldDict = [];
        foreach (var field_ in fields) {
            fieldDict[field_.UniqueName] = field_;
        }
        foreach(var kvp in settings) {
            if (!fieldDict.TryGetValue(kvp.Value.GetUniqueName(), out ISettingField? field)) {
                field = kvp.Value.Create();
                field.Save();
            }
            kvp.Value.SettingField = field;
            settingModels.Add(kvp.Key + "Model", Create(kvp.Key));
        }
    }
    private T GetValue<T>([CallerMemberName] string name = "") where T : notnull {
        return settings[name].GetValueOrDefault<T>();
    }
    private void SetValue(object value, [CallerMemberName] string name = "") {
        Debug.WriteLine($"{name}: {value}");
        settingModels[name].TemporyValue = value;
        settingModels[name].Apply();
        OnPropertyChanged(name);
    }
    private readonly Dictionary<string, SettingFieldModel> settingModels = [];
    private SettingFieldModel Get([CallerMemberName] string name = "") {
        return settingModels[name];
    }
    [RelayCommand]
    public void Apply() {
        foreach (SettingFieldModel field in settingModels.Values) {
            field.Apply();
        }
        foreach (string key in settings.Keys) {
            OnPropertyChanged(key);
        }
        EventSystem.Publish<SettingChangedEventArgs>(this, new());
    }
    [RelayCommand]
    public void Cancel() {
        foreach (SettingFieldModel field in settingModels.Values) {
            field.Cancel();
        }
    }
}
