using MusicEco.Resources.Themes;

namespace MusicEco.Views.Shell;

public partial class App: Application {
    private static IServiceProvider? _services;
    private ResourceDictionary _currentTheme;
    public static IServiceProvider Provider => _services ?? throw new Exception("Object not initilized");
    public App(IServiceProvider servicesProvider) {
        InitializeComponent();
        _services = servicesProvider;
        this._currentTheme = this.Resources.MergedDictionaries.Last();
    }

    protected override Window CreateWindow(IActivationState? activationState) {
        return App.Provider.GetRequiredService<MainWindow>();
    }
    public void SetTheme(ResourceDictionary nextTheme) {
        this.Resources.MergedDictionaries.Remove(this._currentTheme);
        this.Resources.MergedDictionaries.Add(nextTheme);
        this._currentTheme = nextTheme;
    }
    public void SetScale(float scale) {
        DefaultSize defaultSize = [];
        foreach(var (key, value) in defaultSize) {
            if (value is double doubleValue) {
                this.SizeDictionary[key] = (double)scale * doubleValue;
            }
            else if (value is GridLength gridLengthValue) {
                this.SizeDictionary[key] = new GridLength(scale * gridLengthValue.Value, GridUnitType.Absolute);
            }
        }
    }
}