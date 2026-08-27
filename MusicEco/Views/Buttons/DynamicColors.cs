using MusicEco.Core;
using MusicEco.Views.Shell;

namespace MusicEco.Views.Buttons;

public static class DynamicColors {
    public static void Initialize(IServiceProvider provider) {
        var styleService = provider.GetRequiredService<IAppInterfaceService>();
        styleService.ThemeChanged += OnThemeChanged;
        ReloadTheme();
    }
    private static void OnThemeChanged(object? sender, ThemeItem e) {
        ReloadTheme();
    }
    private static void ReloadTheme() {
        var resources = App.Current!.Resources;
        borderColor = (Color)resources["BorderColor"];
        selectedBorderColor = (Color)resources["SelectedBorderColor"];
        highlightColor = (Color)resources["HighlightColor"];
        buttonHighlightColor = (Color)resources["ButtonHighlightColor"];
        itemAltBackgroundColor = (Color)resources["ItemAltBackgroundColor"];
        itemAltBackgroundBrush = new SolidColorBrush(ItemAltBackgroundColor);
    }

    private static Color? borderColor;
    public static Color BorderColor => borderColor ?? throw new NotInitializedException();
    private static Color? selectedBorderColor;
    public static Color SelectedBorderColor => selectedBorderColor ?? throw new NotInitializedException();
    private static Color? highlightColor;
    public static Color HighLightColor => highlightColor ?? throw new NotInitializedException();
    private static Color? buttonHighlightColor;
    public static Color ButtonHighlightColor => buttonHighlightColor ?? throw new NotInitializedException();
    private static Color? itemAltBackgroundColor;
    public static Color ItemAltBackgroundColor => itemAltBackgroundColor ?? throw new NotInitializedException();
    private static Brush? itemAltBackgroundBrush;
    public static Brush ItemAltBackgroundBrush => itemAltBackgroundBrush ?? throw new NotInitializedException();
}
