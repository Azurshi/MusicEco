using Microsoft.Extensions.Logging;
using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Data;
using MusicEco.Image;
using MusicEco.Resources.Themes;
using MusicEco.Services;
using MusicEco.Views.Shell;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SQLiteORM;
using System.Diagnostics;

namespace MusicEco;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        RegisterDependency(builder);
        RegisterStartup();
        RegisterCleanup();
        RegisterLoop();
        return builder.Build();
    }
    private static void RegisterDependency(MauiAppBuilder builder) {
        var services = builder.Services;
        services.RegisterServices();
        services.RegisterShell();
        services.RegisterPages();
        services.RegisterOverlays();
        services.RegisterOthers();

        services.RegisterImage();
        services.RegisterData();
    }
    private static void RegisterStartup() {
        AppLifeCycle.RegisterAppStart(static async (provider) => {
#if WINDOWS
            string savePath = "D:\\Workstation\\Storage\\MusicEco\\Data";
#else
            string savePath = FileSystem.Current.AppDataDirectory;
#endif
            await MusicEco.Data.ServiceRegister.Initialize(provider, savePath);
        });
        AppLifeCycle.RegisterAppStart(static (provider) => {
            var localization = provider.GetRequiredService<ILocalizationService>();
            localization.RegisterMain();
            Localization.Initalize(localization);
        });
        AppLifeCycle.RegisterAppStart(static async (provider) => {
            var iconService = provider.GetRequiredService<IIconService>();
            var setting = provider.GetRequiredService<IAppSetting>();
            await iconService.InitializeDefault(provider);
            var nWorkers = setting.Get(1, SettingFields.IconDecoderNumWorkers);
            var capacity = setting.Get(100, SettingFields.IconDecoderCapacity);
            await iconService.Setup(nWorkers, capacity);
        });
        AppLifeCycle.RegisterAfterUILoaded(static (provider) => {
            // Localization
            NavigateEventArgs args = new(null, PageRoute.None, PageRoute.Home);
            EventSystem.Publish(null, args);
        });

        AppLifeCycle.RegisterAfterUILoaded(static (provider) => {
            // Initialize stack
            var stack = provider.GetRequiredService<NavigationStack>();
            // Initialize PlaybackService
            var playbackService = provider.GetRequiredService<IPlaybackService>();
        });
        AppLifeCycle.RegisterAfterUILoaded(static (provider) => {
            var styleService = provider.GetRequiredService<IStyleService>();
            styleService.Register(new DefaultTheme());
            styleService.Register(new LightTheme());
            styleService.Register(new DarkTheme());
            styleService.LoadLastTheme();
        });
    }
    private static void RegisterCleanup() {
        AppLifeCycle.RegisterAppClose(static (provider) => {
            var icon = provider.GetRequiredService<IIconService>();
            icon.Dispose();
        });
        AppLifeCycle.RegisterAppClose(static (provider) => {
            var db = provider.GetRequiredService<DatabaseContextAsync>();
            db.Dispose(true);
        });
        AppLifeCycle.RegisterAppClose(static (provider) => {
            var player = provider.GetRequiredService<IPlayerController>();
            player.Dispose();
        });
    }
    private static void RegisterLoop() {
        AppLifeCycle.RegisterLoop("IconCache", static (provider) => {
            var cache = provider.GetRequiredService<IIconService>();
            //Debug.WriteLine("Loop: Try compact cache");
            cache.Compact();
        }, TimeSpan.FromSeconds(1));
    }
}
