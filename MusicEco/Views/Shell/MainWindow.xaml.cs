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
    private IPageResolver? _pageResolver;
    protected override async void OnCreated() {
        base.OnCreated();
        await AppLifeCycle.StartApp();
        var provider = AppLifeCycle.Provider;
        // Localization
        var localizationService = provider.GetRequiredService<ILocalizationService>();
        this.L = localizationService.Get(typeof(MainWindow));
        localizationService.LanguageChanged += (_, _) => this.LoadLanguage();
        this.LoadLanguage();
        // Navigation
        this._pageResolver = provider.GetRequiredService<IPageResolver>();
        // UIContent
        this.NavigationBarHost.Content = provider.GetRequiredService<NavigationBar>();
        this.ControlBarHost.Content = provider.GetRequiredService<ControlBar>();
        this.OverlayHost.Content = provider.GetRequiredService<AppOverlay>();
        // Landing page
        EventSystem.Connect<NavigateEventArgs>(OnNavigate);

        await AppLifeCycle.AfterUILoaded();

    }
    protected override async void OnActivated() {
        base.OnActivated();
        Debug.WriteLine("Acitvated");
    }
    protected override void OnDestroying() {
        base.OnDestroying();
        AppLifeCycle.CloseApp();
    }
    private async void OnNavigate(object? sender, NavigateEventArgs e) {
        if (this._pageResolver == null) {
            throw new Exception("Not initialized");
        }
        if (ViewHost.Content != null && ViewHost.Content.BindingContext is INavigationAware fromNavigationAware) {
            await fromNavigationAware.OnNavigatedFrom(e);
        }
        var nextPage = this._pageResolver.GetPage(e.ToPage);
        if (nextPage.BindingContext is INavigationAware toNavigationAware) {
            await toNavigationAware.OnNavigateTo(e);
        }
        ViewHost.Content = nextPage;
    }

    private void Button_Clicked(object sender, EventArgs e) {
        Debug.WriteLine(FileSystem.AppDataDirectory);
    }
}