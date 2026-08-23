using MusicEco.Core;
using MusicEco.Services;
using MusicEco.ViewModels.Pages;
using System.Diagnostics;

namespace MusicEco.Views.Shell;

public partial class MainPage: ContentPage {
    private readonly IServiceProvider _provider;
    private readonly IPageResolver _pageResolver;
    private readonly IAppInterfaceService _interfaceService;
    public MainPage(IServiceProvider serviceProvider, IPageResolver pageResolver, IAppInterfaceService appInterfaceService) {
        InitializeComponent();
        this._provider = serviceProvider;
        this._pageResolver = pageResolver;
        this._interfaceService = appInterfaceService;
        this.Loaded += this.MainPage_Loaded;
    }

    private async void MainPage_Loaded(object? sender, EventArgs e) {
        Debug.WriteLine("Main page loaded");
        var interaceService = this._provider.GetRequiredService<IAppInterfaceService>();
        interaceService.ScaleChanged += this.InteraceService_ScaleChanged;
        interaceService.OrientationChanged += this.InteraceService_OrientationChanged;
        // Landing page
        EventSystem.Connect<NavigateEventArgs>(OnNavigate);

        this.LoadLayout();
        await AppLifeCycle.AfterUILoaded();
    }

    private void InteraceService_OrientationChanged(object? sender, DisplayOrientation e) {
        this.LoadLayout();
    }

    private void InteraceService_ScaleChanged(object? sender, float e) {
        if (this.Container == null) {
            throw new Exception("Not initialized");
        }
        this.LoadGridDefinition(this.Container);
    }
    private void LoadGridDefinition(Grid container) {
        DisplayOrientation orientation = this._interfaceService.GetOrientation();
        double navigationBarSize = Utility.GetResource<double>("NavigationBarSize");
        double progressBarSize = Utility.GetResource<double>("ProgressBarSize");
        double spacing = 4;
        if (orientation == DisplayOrientation.Landscape) {
            RowDefinitionCollection rowDefinitions = new([
                new RowDefinition(new GridLength(1, GridUnitType.Star)),
                new RowDefinition(new GridLength(navigationBarSize + progressBarSize + spacing, GridUnitType.Absolute))
            ]);
            ColumnDefinitionCollection columnDefinitions = new([
                new ColumnDefinition(new GridLength(navigationBarSize, GridUnitType.Absolute)),
            new ColumnDefinition(new GridLength(1, GridUnitType.Star))
                ]);
            container.RowDefinitions = rowDefinitions;
            container.ColumnDefinitions = columnDefinitions;
        }
        else {
            RowDefinitionCollection rowDifinitions = new([
                new RowDefinition(new GridLength(1, GridUnitType.Star)),
                new RowDefinition(new GridLength(navigationBarSize * 2 + progressBarSize + spacing * 0, GridUnitType.Absolute)),
                new RowDefinition(new GridLength(navigationBarSize, GridUnitType.Absolute))
                ]);
            container.RowDefinitions = rowDifinitions;
        }
    }
    private void LoadLayout() {
        DisplayOrientation orientation = this._interfaceService.GetOrientation();
        ContentView? oldViewHost = null;
        if (this.Content is Grid oldContainer) {
            oldViewHost = oldContainer.FindByName<ContentView>("ViewHost");
            foreach (var child in oldContainer.Children) {
                if (child is ContentView childView && childView != oldViewHost) {
                    childView.Content = null;
                }
            }
        }
        Grid container = orientation switch {
            DisplayOrientation.Landscape => this.LoadTemplate<Grid>("LandscapeLayout"),
            DisplayOrientation.Portrait => this.LoadTemplate<Grid>("PortraitLayout"),
            _ => throw new ValueNotExistsExeption()
        };
        var navigationBarHost = container.FindByName<ContentView>("NavigationBarHost");
        var controlBarHost = container.FindByName<ContentView>("ControlBarHost");
        var overlayHost = container.FindByName<ContentView>("OverlayHost");
        var viewHost = container.FindByName<ContentView>("ViewHost");

        if (oldViewHost != null) {
            var pageContent = oldViewHost.Content;
            oldViewHost.Content = null;
            viewHost.Content = pageContent;
        }

        navigationBarHost.Content = this._provider.GetRequiredService<NavigationBar>();
        controlBarHost.Content = this._provider.GetRequiredService<ControlBar>();
        overlayHost.Content = this._provider.GetRequiredService<AppOverlay>();
        this.Container = container;
        this.ViewHost = viewHost;
        this.Content = container;
        LoadGridDefinition(container);
    }
    private ContentView? ViewHost;
    private Grid? Container;
    private async void OnNavigate(object? sender, NavigateEventArgs e) {
        if (this.ViewHost == null) {
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
}