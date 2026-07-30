using MusicEco.Core.Types;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace MusicEco.Core.Services;

public class AssemblyLocalization {
    private readonly ResourceManager _resourceManager;
    public string this[string key] => _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? string.Empty;
    public string Format(string key, params string[] args) {
        return string.Format(this[key], args);
    }
    public AssemblyLocalization(ResourceManager resourceManager) {
        _resourceManager = resourceManager;
    }
}
public interface ILocalizationService {
    public event EventHandler LanguageChanged;
    public Task SetLanguage(string cultureCode, object? caller = null);
    public Task<string> GetCurrentLanguageCode();
    public AssemblyLocalization Get(Type type);
    public void RegisterResource(Assembly assembly, ResourceManager resourceManager);
}