using MusicEco.Core.Services;
using MusicEco.Views.Shell;

namespace MusicEco.Services;

public class StyleService: IStyleService {
    private static App CurrentApp => (App?)Application.Current ?? throw new NullReferenceException();
    private readonly IAppSetting _setting;
    private readonly Dictionary<string, ITheme> _themes;

    public event EventHandler? ThemeChanged;

    private string LastThemeId {
        get => this._setting.Get("default", "App.CurrentThemeId");
        set => this._setting.Set(value, "App.CurrentThemeId");
    }
    public StyleService(IAppSetting appSetting) {
        this._setting = appSetting;
        this._themes = [];
    }
    public void SetTheme(string themeId) {
        if (this._themes.TryGetValue(themeId, out var theme)) {
            CurrentApp.SetTheme(theme.GetResources());
            this.LastThemeId = theme.Id;
            this.ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public void LoadLastTheme() {
        var lastThemeId = this.LastThemeId;
        if (this._themes.TryGetValue(lastThemeId, out var theme)) {
            CurrentApp.SetTheme(theme.GetResources());
            this.ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public List<ITheme> GetAll() {
        return this._themes.Values.OrderBy(t => t.Name).ToList();
    }

    public void Register(ITheme theme) {
        if (!this._themes.ContainsKey(theme.Id)) {
            this._themes[theme.Id] = theme;
        }
    }

    public string GetCurrentThemeId() {
        return LastThemeId;
    }

    public string GetCurrentThemeName() {
        if (this._themes.TryGetValue(this.LastThemeId, out var theme)) {
            return theme.Name;
        }
        else {
            return "Unknown";
        }
    }
}
