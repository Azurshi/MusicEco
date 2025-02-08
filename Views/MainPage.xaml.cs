using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Global.AbstractLayers;

namespace MusicEco;
public partial class MainPage : ContentPage
{
    public MainPage() {
        InitializeComponent();
        Global.EventSystem.Publish(Signal.System_Before_UIStart, null, EventArgs.Empty);
#if WINDOWS
        var window = Application.Current!.Windows.FirstOrDefault();
        if (window != null) {
            window.Width = 1280;
            window.Height = 720;
        }
#endif
        this.SizeChanged += OnSizeChanged;
        Global.EventSystem.Connect(Signal.System_Before_UIStart,
            (s, e) => OnSizeChanged(this, EventArgs.Empty));

        Application.Current!.Windows[0].Stopped += MainPage_Stopped;

    }
    private void MainPage_Stopped(object? sender, EventArgs e) {
        Common.Value.System.AppRunning = false;
        DataStorage.ForceSave();
    }
    private void OnSizeChanged(object? sender, EventArgs e) {
        if (sender != null) {
            double width = this.Width;
            double height = this.Height;
            Common.Value.UI.SetNumRow(height, 12);
            Vector2IEventArgs args = new((int)width, (int)height);
            EventSystem.Publish(Signal.UI_WindowSize_Changed, this, args);
        }
    }
    #region Wiring
    private void Tablist_TabSwitched(object? sender, Views.Components.TabSwitchedEventArgs e) => Sections.On_TabList_Switched(sender, e);
    #endregion
}
