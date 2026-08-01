using Microsoft.Extensions.DependencyInjection;

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
}