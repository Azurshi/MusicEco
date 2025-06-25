using Domain.EventSystem;
using MusicEco.ViewModels;

namespace MusicEco;
public partial class AppShell : Shell
{
    public static double ScreenHeight = 0;
    public class WindowSizeChangedEventArgs(double width, double height): EventArgs {
        public double Width { get; set; } = width;
        public double Height { get; set; } = height;
    }
    public AppShell()
    {
        InitializeComponent();
        this.SizeChanged += AppShell_SizeChanged;
#if WINDOWS
        var window = Application.Current!.Windows.FirstOrDefault();
        if (window != null) {
            window.Width = 1280;
            window.Height = 720;
        }
#endif
        this.Navigated += AppShell_Navigated;
        EventSystem.Publish<AppSettingModel.SettingChangedEventArgs>(this, new());

    }
    private Page? previousPage;
    private void AppShell_Navigated(object? sender, ShellNavigatedEventArgs e) {
        if (previousPage != null) {
            previousPage.SizeChanged -= AppShell_SizeChanged;
        }
        Page current = Shell.Current.CurrentPage;
        current.SizeChanged += AppShell_SizeChanged;
        previousPage = current;
    }


    private void AppShell_SizeChanged(object? sender, EventArgs e) {
        if (sender != null) {
            Page page = (Page)sender;
            double width = page.Width;
            double height = page.Height;
            ScreenHeight = height;
            EventSystem.Publish<WindowSizeChangedEventArgs>(this, new(width, height));
        }
    }
}
