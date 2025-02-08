using MusicEco.Common.Events;
using MusicEco.Global;

namespace MusicEco.Views.Widgets;

public partial class VolumeSlider : ContentView
{
    private bool _isDragging = false;
    private double _lastPercent;
    private double _lastDragged;
    public VolumeSlider() {
        InitializeComponent();
        MusicPlayer.VolumeChanged.Connect(OnMusicPlayer_VolumeChanged);

    }
    public void Refresh() {
        UpdateProgress(MusicPlayer.Volume);
    }
    private void OnMusicPlayer_VolumeChanged(object? sender, FloatEventArgs e) {
        UpdateProgress(e.Value);
    }
    private void UpdateProgress(float percent) {
        if (!_isDragging && percent >= 0 && percent <= 1) {
            double dPercent = (double)percent;
#if WINDOWS
            _lastPercent = HolderLayout.Width * dPercent;
            AbsoluteLayout.SetLayoutBounds(UnderLabel, new Rect(0, 0, HolderLayout.Width, HolderLayout.Height));
            AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, _lastPercent, HolderLayout.Height));
            AbsoluteLayout.SetLayoutBounds(Icon,
                new Rect(
                    _lastPercent - Common.Setting.ProgressIconSize.X / 2,
                    HolderLayout.Height / 2 - Common.Setting.ProgressIconSize.Y / 2,
                    Common.Setting.ProgressIconSize.X,
                    Common.Setting.ProgressIconSize.Y
                    )
                );
#else
            _lastPercent = HolderLayout.Height * dPercent;
            AbsoluteLayout.SetLayoutBounds(UnderLabel, new Rect(0, 0, HolderLayout.Width, HolderLayout.Height));
            AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, HolderLayout.Width, _lastPercent));
            AbsoluteLayout.SetLayoutBounds(Icon,
                new Rect(
                    HolderLayout.Width / 2 - Common.Setting.ProgressIconSize.X / 2,
                    _lastPercent - Common.Setting.ProgressIconSize.Y / 2,
                    Common.Setting.ProgressIconSize.X,
                    Common.Setting.ProgressIconSize.Y
                    )
                );
#endif
        }
    }
    private void Icon_PanUpdated(object sender, PanUpdatedEventArgs e) {
        if (e.StatusType == GestureStatus.Started) {
            _isDragging = true;
        }
        else if (e.StatusType == GestureStatus.Running) {
#if WINDOWS
            double x = e.TotalX;
            _lastDragged = _lastPercent + x;
            AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, _lastDragged, HolderLayout.Height));
            AbsoluteLayout.SetLayoutBounds(Icon,
                new Rect(
                    _lastDragged - Common.Setting.ProgressIconSize.X / 2,
                    HolderLayout.Height / 2 - Common.Setting.ProgressIconSize.Y / 2,
                    Common.Setting.ProgressIconSize.X,
                    Common.Setting.ProgressIconSize.Y
                    )
                );
#else
            double y = e.TotalY;
            _lastDragged = _lastPercent + y;
            AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, HolderLayout.Width, _lastPercent));
            AbsoluteLayout.SetLayoutBounds(Icon,
                new Rect(
                    HolderLayout.Width / 2 - Common.Setting.ProgressIconSize.X / 2,
                    _lastDragged - Common.Setting.ProgressIconSize.Y / 2,   
                    Common.Setting.ProgressIconSize.X,
                    Common.Setting.ProgressIconSize.Y
                    )
                );
#endif
        }
        else if (e.StatusType == GestureStatus.Completed) {
            _isDragging = false;
#if WINDOWS
            float percent = (float)(_lastDragged / HolderLayout.Width);
#else
            float percent = (float)(_lastDragged / HolderLayout.Height);
#endif
            MusicPlayer.ChangeVolume(percent);
        }
    }
}