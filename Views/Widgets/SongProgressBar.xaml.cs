using MusicEco.Common.Events;
using MusicEco.Global;

namespace MusicEco.Views.Widgets;

public partial class SongProgressBar : ContentView
{
    private bool _isDragging = false;
    private double _lastProgressed;
    private double _lastDragged;
    public SongProgressBar()
	{
		InitializeComponent();
        MusicPlayer.ProgressChanged.Connect(OnMusicPlayer_ProgressChanged);
	}
    private void OnMusicPlayer_ProgressChanged(object? sender, FloatEventArgs e) {
        UpdateProgress(e.Value);
    }
	private void UpdateProgress(float percent) {
        if (!_isDragging && percent >= 0 && percent <= 1) {
            double dPercent = (double)percent;
            _lastProgressed = HolderLayout.Width * dPercent;
            AbsoluteLayout.SetLayoutBounds(UnderLabel, new Rect(0, 0, HolderLayout.Width, HolderLayout.Height));
            AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, _lastProgressed, HolderLayout.Height));
            AbsoluteLayout.SetLayoutBounds(Icon,
                new Rect(
                    _lastProgressed - Common.Setting.ProgressIconSize.X / 2,
                    HolderLayout.Height / 2 - Common.Setting.ProgressIconSize.Y / 2,
                    Common.Setting.ProgressIconSize.X,
                    Common.Setting.ProgressIconSize.Y
                    )
                );
        }
    }
    private async void Icon_PanUpdated(object sender, PanUpdatedEventArgs e) {
        if (e.StatusType == GestureStatus.Started) {
            _isDragging = true;
        }
        else if (e.StatusType == GestureStatus.Running) {
            double x = e.TotalX;
            _lastDragged = _lastProgressed + x;
            AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, _lastDragged, HolderLayout.Height));
            AbsoluteLayout.SetLayoutBounds(Icon,
                new Rect(
                    _lastDragged - Common.Setting.ProgressIconSize.X / 2,
                    HolderLayout.Height / 2 - Common.Setting.ProgressIconSize.Y / 2,
                    Common.Setting.ProgressIconSize.X,
                    Common.Setting.ProgressIconSize.Y
                    )
                );
        }
        else if (e.StatusType == GestureStatus.Completed) {
            _isDragging = false;
            float percent = (float)(_lastDragged / HolderLayout.Width);
            await MusicPlayer.ChangeProgress(percent);
        }
    }
}