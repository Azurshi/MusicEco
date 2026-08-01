using MusicEco.Core.Services;
using MusicEco.ViewModels.Pages;
using MusicEco.ViewModels.Shell;
using MusicEco.Views.Pages;
using MusicEco.Views.Shell;

namespace MusicEco.Services;

public static class ServiceRegister {
    public static IServiceCollection RegisterServices(this IServiceCollection services) {
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<NavigationStack>();
        services.AddSingleton<IPageResolver, PageResolver>();
        services.AddSingleton<IPageRouteRegistry, PageRegistry>();
        services.AddSingleton<IIconService, IconService>();
        return services;
    }
    public static IServiceCollection RegisterShell(this IServiceCollection services) {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<NavigationBar>();
        services.AddSingleton<ControlBar>();
        services.AddSingleton<ControlBarViewModel>();
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
        services.AddSingleton<SearchPage>();
        services.AddSingleton<SearchPageViewModel>();
        services.AddTransient<UserPage>();
        services.AddSingleton<UserPageViewModel>();
        services.AddTransient<SettingPage>();
        services.AddSingleton<SettingPageViewModel>();

        services.AddTransient<PlaylistPage>();
        services.AddSingleton<PlaylistPageViewModel>();
        return services;
    }
    public static IServiceCollection RegisterOverlays(this IServiceCollection services) {
        return services;
    }
    public static IServiceCollection RegisterOthers(this IServiceCollection services) {
        return services;
    }
}