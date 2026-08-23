using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Services;
using MusicEco.ViewModels.Pages;
using MusicEco.Views.Pages;
using System.Diagnostics;

namespace MusicEco.Views.Shell;

public partial class MainWindow: Window {
    public AssemblyLocalization? L { get; private set; }
    public MainWindow() {
        InitializeComponent();
    }
    private void LoadLanguage() {
        this.Title = L?["AppTitle"];
    }
    protected override async void OnCreated() {
        base.OnCreated();
        await AppLifeCycle.StartApp();
        var provider = AppLifeCycle.Provider;
        // Localization
        var localizationService = provider.GetRequiredService<ILocalizationService>();
        this.L = localizationService.Get(typeof(MainWindow));
        localizationService.LanguageChanged += (_, _) => this.LoadLanguage();
        this.LoadLanguage();

        this.Page = provider.GetRequiredService<MainPage>();
    }

    protected override async void OnActivated() {
        base.OnActivated();
        Debug.WriteLine("Activated");
    }

    protected override void OnDestroying() {
        base.OnDestroying();
        AppLifeCycle.CloseApp();
    }
}