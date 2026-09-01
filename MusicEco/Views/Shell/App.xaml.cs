using MusicEco.Resources.Styles;

namespace MusicEco.Views.Shell;

public partial class App: Application {
    private static IServiceProvider? _services;
    public static IServiceProvider Provider => _services ?? throw new Exception("Object not initilized");
    public App(IServiceProvider servicesProvider) {
        InitializeComponent();
        _services = servicesProvider;
    }

    protected override Window CreateWindow(IActivationState? activationState) {
        return App.Provider.GetRequiredService<MainWindow>();
    }
    public void SetTheme(ResourceDictionary nextTheme) {
        foreach(var (key, value) in nextTheme) {
            this.PalletDictionary[key] = value;
        }
    }
    public void SetScale(float scale) {
        var defaultDict = new WidgetSize();
        foreach(var (key, value) in defaultDict) {
            if (value is double doubleValue) {
                this.SizeDictionary[key] = (double)scale * doubleValue;
            }
            else if (value is int intValue) {
                this.SizeDictionary[key] = (int)(scale * intValue);
            }
            else if (value is Thickness thickness) {
                this.SizeDictionary[key] = new Thickness(thickness.Left * scale, thickness.Top * scale, thickness.Right * scale, thickness.Bottom * scale);
            }
            else if (value is GridLength gridLength) {
                this.SizeDictionary[key] = new GridLength(scale * gridLength.Value, GridUnitType.Absolute);
            }
        }
    }
}