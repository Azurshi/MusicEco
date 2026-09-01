using MusicEco.Core.Services;
using MusicEco.Views.Shell;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace MusicEco.Services;

public class LocalizationService: ILocalizationService {
    public event EventHandler? LanguageChanged;
    private readonly IAppSetting _setting;
    private readonly Dictionary<Assembly, AssemblyLocalization> _resourceMap;
    private readonly Dictionary<Type, Assembly> _typeCache;
    private const string StorageFieldName = "Language";
    public LocalizationService(IAppSetting setting) {
        this._setting = setting;
        this._resourceMap = [];
        this._typeCache = [];
    }

    public AssemblyLocalization Get(Type type) {
        if (!this._typeCache.TryGetValue(type, out var assembly)) {
            assembly = type.Assembly;
            this._typeCache[type] = assembly;
        }
        if (_resourceMap.TryGetValue(assembly, out var resourceManager)) {
            return resourceManager;
        } else {
            throw new KeyNotFoundException($"Resource not found for {assembly.FullName}");
        }
    }

    public string GetCurrentLanguageCode() {
        return this._setting.Get("en", StorageFieldName);
    }

    public void RegisterResource(Assembly assembly, ResourceManager resourceManager) {
        if (!this._resourceMap.TryGetValue(assembly, out var _)) {
            this._resourceMap[assembly] = new(resourceManager);
        }
    }

    public void SetLanguage(string cultureCode, object? caller = null) {
        Debug.WriteLine($"Language changed to {cultureCode}");
        var cultureInfo = new CultureInfo(cultureCode);
        CultureInfo.CurrentUICulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        this._setting.Set(cultureCode, StorageFieldName);
        LanguageChanged?.Invoke(caller, EventArgs.Empty);
    }
}
public static class Localization {
    private static ILocalizationService? _service;
    public  static ILocalizationService Service => _service ?? throw new NullReferenceException();
    private static AssemblyLocalization? _instance;
    public static AssemblyLocalization Instance => _instance ?? throw new NullReferenceException();
    public static AssemblyLocalization L => Instance;
    public static void Initalize(ILocalizationService service) {
        _service = service;
        _instance = _service.Get(typeof(MainWindow));
        service.SetLanguage(_service.GetCurrentLanguageCode());
    }
    public static ILocalizationService RegisterMain(this ILocalizationService localizationService) {
        localizationService.RegisterResource(typeof(MainWindow).Assembly, MusicEco.Resources.Localization.Text.ResourceManager);
        return localizationService;
    }
}