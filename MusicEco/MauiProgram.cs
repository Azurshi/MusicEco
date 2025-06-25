using CommunityToolkit.Maui;
using DataStorage.DataAccess;
using DataStorage.Models;
using Domain.DataAccess;
using Domain.EventSystem;
using Domain.Models;
using Microsoft.Extensions.Logging;
#if ANDROID
using MusicEco.Platforms.Android;
#endif
using MusicEco.ViewModels;
using MusicEco.ViewModels.Components;
using MusicEco.ViewModels.DetailPages;
using MusicEco.ViewModels.Pages;
using MusicEco.Views.DetailPages;
using MusicEco.Views.Pages;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MusicEco;
public class LoadErrorHandler : IErrorHandler {
    public void HandleError(Exception ex) {
        Debug.WriteLine("---------------- Failed to load data ------------");
        Debug.WriteLine($"Error: {ex}");
        //IScanner scanner = new Scanner();
        //scanner.DeleteAllData(); // Not work
        Thread.Sleep(1000);
    }
}
public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        List<Type> staticInitializerResidentTypes = [
            typeof(Domain.EventSystem.EventSystem), typeof(MauiProgram), typeof(DataStorage.Models.BaseModel),
            typeof(AudioPlayer.AudioPlayer)
        ];
        try {
            DataStorage.Serialization.InitializeAndLoad();
        }
        catch (Exception ex) {
            Debug.WriteLine("------------------- ERROR ---------------");
            Debug.WriteLine(ex.Message);
            IScanner scanner = new Scanner();
            scanner.DeleteAllData(); // is this work ?
        }

        DataStorage.Serialization.SaveTask().FireAndForgetAsync();

        StaticInitializerAttribute.StaticInitialize(staticInitializerResidentTypes); // Current playing song bypass through this
        EventSystem.PrintBlockedEvents.Add(typeof(Domain.Models.DataSavedEventArgs));
        EventSystem.PrintBlockedEvents.Add(typeof(PlayerProgressChangedEventArgs));

        #region DependencyInjection
        builder.Services.AddSingleton<IScanner, Scanner>();
        builder.Services.AddSingleton<IModelGetter, ModelGetter>();
        builder.Services.AddTransient<ISongModel, SongModel>();
        builder.Services.AddTransient<IPlaylistModel, PlaylistModel>();
        builder.Services.AddTransient<ISettingField, SettingFieldModel>();
        builder.Services.AddTransient<IFileModel, FileModel>();
        builder.Services.AddTransient<IFolderModel, FolderModel>();

        builder.Services.AddSingleton<GlobalModel>();
        builder.Services.AddSingleton<NavigationBarModel>();
        builder.Services.AddSingleton<ControlBarModel>();
        builder.Services.AddSingleton<UserPreviewModel>();
        builder.Services.AddSingleton<InfoPreviewModel>();

        builder.Services.AddSingletonWithShellRoute<OverviewPage, OverviewPageModel>("overview");
        builder.Services.AddSingletonWithShellRoute<QueuePage, QueuePageModel>("queue");
        builder.Services.AddSingletonWithShellRoute<PlaylistPage, PlaylistPageModel>("playlist");
        builder.Services.AddSingletonWithShellRoute<PlaylistDetailPage, PlaylistDetailPageModel>("playlist_detail");
        builder.Services.AddSingletonWithShellRoute<AlbumPage, AlbumPageModel>("album");
        builder.Services.AddSingletonWithShellRoute<AlbumDetailPage, AlbumDetailPageModel>("album_detail");
        builder.Services.AddSingletonWithShellRoute<ExplorerPage, ExplorerPageModel>("explorer");
        builder.Services.AddSingletonWithShellRoute<SearchPage, SearchPageModel>("search");
        builder.Services.AddSingletonWithShellRoute<SettingPage, SettingPageModel>("setting");

        builder.Services.AddSingletonWithShellRoute<UserPage, UserPageModel>("user");
        builder.Services.AddSingletonWithShellRoute<FavouritePage, FavouritePageModel>("favourite");
        builder.Services.AddSingletonWithShellRoute<PlaycountPage, PlaycountPageModel>("playcount");
        builder.Services.AddSingletonWithShellRoute<ScanPage, ScanPageModel>("scan");

        #endregion
        IServiceProvider service = builder.Services.BuildServiceProvider();
        IServiceAccess._service = service;
        Startup(service).FireAndForgetAsync();
        return builder.Build();
    }
    private static async Task Startup(IServiceProvider service) {
        ScanPageModel model = service.GetRequiredService<ScanPageModel>();
#if ANDROID
        await UriUtility.RequestPermission();
#endif
        await model.ScanMusic();
    }

}
