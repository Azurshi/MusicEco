namespace MusicEco.Services;

internal partial class AppInterfaceService {
    private const string DefaultTheme = "default";
    private const string ThemeStorageField = "AppTheme";
    public event EventHandler<ThemeItem>? ThemeChanged;
    private readonly Dictionary<string, ITheme> _registeredThemes = [];
    private string GetThemeName(string themeId) {
        if (this._registeredThemes.TryGetValue(themeId, out var theme)) {
            return theme.Name;
        }
        throw new KeyNotFoundException(nameof(themeId));
    }
    public ThemeItem GetTheme() {
        var id = this._setting.Get(DefaultTheme, ThemeStorageField);
        return new(id, GetThemeName(id));
    }
    public IReadOnlyList<ThemeItem> GetThemes() {
        List<ThemeItem> items = [];
        foreach(var theme in this._registeredThemes.Values) {
            items.Add(new(theme.Id, theme.Name));
        }
        items = items.OrderBy(i => i.Text).ToList();
        return items;
    }
    public void SetTheme(string themeId) {
        if (this.GetTheme().Id != themeId 
            && this._registeredThemes.TryGetValue(themeId, out var themeFactory)) {
            this.App.SetTheme(themeFactory.GetResources());
            var name = this.GetThemeName(themeId);
            this._setting.Set(themeId, ThemeStorageField);
            this.ThemeChanged?.Invoke(this, new(themeId, name));
        }
    }
    public void LoadLastTheme() {
        var lastTheme = this.GetTheme();
        if (this._registeredThemes.TryGetValue(lastTheme.Id, out var themeFactory)) {
            this.App.SetTheme(themeFactory.GetResources());
            this.ThemeChanged?.Invoke(this, lastTheme);
        }
    }
    public void RegisterTheme(ITheme theme) {
        if (!this._registeredThemes.ContainsKey(theme.Id)) {
            this._registeredThemes[theme.Id] = theme;
        }
    }
}
