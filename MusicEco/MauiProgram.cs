using Microsoft.Extensions.Logging;
using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Data;
using MusicEco.Image;
using MusicEco.Services;
using MusicEco.Views.Shell;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SQLiteORM;

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
#if WINDOWS
        string savePath = "D:\\Workstation\\Storage\\MusicEco\\Data";
#else
        string savePath = FileSystem.Current.AppDataDirectory;
#endif
        AppLifeCycle.RegisterAppStart(async (provider) => {
            await MusicEco.Data.ServiceRegister.Initialize(provider, savePath);
        });
        AppLifeCycle.RegisterAppStart((provider) => {
            var localization = provider.GetRequiredService<ILocalizationService>();
            localization.RegisterMain();
            Localization.Initalize(localization);
        });
        AppLifeCycle.RegisterAppStart(async (provider) => {
            var iconService = provider.GetRequiredService<IIconService>();
            var setting = provider.GetRequiredService<IAppSetting>();
            await iconService.InitializeDefault(provider);
            var nWorkers = setting.Get(1, SettingFields.IconDecoderNumWorkers);
            var capacity = setting.Get(100, SettingFields.IconDecoderCapacity);
            await iconService.Setup(nWorkers, capacity);
        });
        AppLifeCycle.RegisterAfterUILoaded((provider) => {
            // Initialize stack
            var stack = provider.GetRequiredService<NavigationStack>();
        });
        AppLifeCycle.RegisterAfterUILoaded((provider) => {
            // Localization
            NavigateEventArgs args = new(null, PageRoute.None, PageRoute.Home);
            EventSystem.Publish(null, args);
        });
    }
    private static void RegisterCleanup() {
        AppLifeCycle.RegisterAppClose((provider) => {
            var db = provider.GetRequiredService<DatabaseContextAsync>();
            db.Dispose(true);
        });
        AppLifeCycle.RegisterAppClose((provider) => {
            var player = provider.GetRequiredService<IPlayerController>();
            player.Dispose();
        });
    }
}
