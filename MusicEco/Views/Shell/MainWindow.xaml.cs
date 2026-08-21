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

        var interaceService = provider.GetRequiredService<IAppInterfaceService>();
        interaceService.ScaleChanged += this.InteraceService_ScaleChanged;
        await AppLifeCycle.AfterUILoaded();

    }

    private void InteraceService_ScaleChanged(object? sender, float e) {
        RowDefinitionCollection rowDefinitions = new([
            new RowDefinition(new GridLength(1, GridUnitType.Star)),
            new RowDefinition(new GridLength(Utility.GetResource<double>("ControlBarSize"), GridUnitType.Absolute))
            ]);
        ColumnDefinitionCollection columnDefinitions = new([
            new ColumnDefinition(new GridLength(Utility.GetResource<double>("NavigationBarSize"), GridUnitType.Absolute)),
            new ColumnDefinition(new GridLength(1, GridUnitType.Star))
            ]);
        this.Container.RowDefinitions = rowDefinitions;
        this.Container.ColumnDefinitions = columnDefinitions;
    }

    protected override async void OnActivated() {
        base.OnActivated();
        Debug.WriteLine("Activated");
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