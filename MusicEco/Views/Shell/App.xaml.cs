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
}