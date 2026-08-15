using MusicEco.Core.Services;
using MusicEco.ViewModels.Overlays;
using MusicEco.ViewModels.Pages;
using MusicEco.ViewModels.Pages.Settings;
using MusicEco.ViewModels.Pages.Users;
using MusicEco.ViewModels.Shell;
using MusicEco.Views.Overlays;
using MusicEco.Views.Pages;
using MusicEco.Views.Pages.Settings;
using MusicEco.Views.Pages.Users;
using MusicEco.Views.Shell;

namespace MusicEco.Services;

public static class ServiceRegister {
    public static IServiceCollection RegisterServices(this IServiceCollection services) {
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<NavigationStack>();
        services.AddSingleton<IPageResolver, PageResolver>();
        services.AddSingleton<IPageRouteRegistry, PageRegistry>();
        services.AddSingleton<IIconService, IconService>();
        services.AddSingleton<IPlayerController, PlayerController>();
        services.AddSingleton<IPlaybackService, PlaybackService>();
        services.AddSingleton<IOverlayService>(provider => provider.GetRequiredService<AppOverlay>());
        return services;
    }
    public static IServiceCollection RegisterShell(this IServiceCollection services) {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<NavigationBar>();
        services.AddSingleton<ControlBar>();
        services.AddSingleton<ControlBarViewModel>();
        services.AddSingleton<AppOverlay>();
        return services;
    }
    public static IServiceCollection RegisterPages(this IServiceCollection services) {
        services.AddSingleton<HomePage>();
        services.AddSingleton<HomePageViewModel>();
        services.AddSingleton<QueuePage>();
        services.AddSingleton<QueuePageViewModel>();
        services.AddSingleton<AlbumPage>();
        services.AddSingleton<AlbumPageViewModel>();
        services.AddSingleton<ExplorerPage>();
        services.AddSingleton<ExplorerPageViewModel>();
        services.AddSingleton<ExplorerTreePage>();
        services.AddSingleton<ExplorerTreePageViewModel>();
        services.AddSingleton<SearchPage>();
        services.AddSingleton<SearchPageViewModel>();
        services.AddTransient<UserPage>();
        services.AddSingleton<UserPageViewModel>();
        services.AddTransient<SettingPage>();
        services.AddSingleton<SettingPageViewModel>();

        services.AddSingleton<QueueDetailPage>();
        services.AddSingleton<QueueDetailPageViewModel>();
        services.AddSingleton<AlbumDetailPage>();
        services.AddSingleton<AlbumDetailPageViewModel>();

        services.AddSingleton<PlaylistPage>();
        services.AddSingleton<PlaylistPageViewModel>();
        services.AddSingleton<PlaylistDetailPage>();
        services.AddSingleton<PlaylistDetailPageViewModel>();
        services.AddTransient<FavouritePage>();
        services.AddSingleton<FavouritePageViewModel>();

        services.AddTransient<LanguageSettingPage>();
        services.AddTransient<LanguageSettingPageViewModel>();
        return services;
    }
    public static IServiceCollection RegisterOverlays(this IServiceCollection services) {
        services.AddTransient<AddToQueueOverlay>();
        services.AddTransient<AddToQueueOverlayViewModel>();
        services.AddTransient<AddToPlaylistOverlay>();
        services.AddTransient<AddToPlaylistOverlayViewModel>();
        services.AddTransient<AudioInfoOverlay>();
        services.AddTransient<AudioInfoOverlayViewModel>();
        services.AddTransient<CreateNewPlaylistOverlay>();
        services.AddTransient<CreateNewPlaylistOverlayViewModel>();
        return services;
    }
    public static IServiceCollection RegisterOthers(this IServiceCollection services) {
        return services;
    }
}