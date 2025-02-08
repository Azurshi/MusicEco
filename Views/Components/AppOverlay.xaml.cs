using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global;
using System.Diagnostics;

namespace MusicEco.Views.Components;

public partial class AppOverlay : ContentView
{
	public AppOverlay()
	{
		InitializeComponent();
        EventSystem.Connect<OptionMenuEventArgs>(Signal.Overlay_StartOptionMenu, OnOptionMenuStart);
        EventSystem.Connect(Signal.Overlay_StopOptionMenu, OnOptionMenuStop);
        EventSystem.Connect<FormMenuEventArgs>(Signal.Overlay_StartFormRequest, OnFormMenuStart);
        EventSystem.Connect(Signal.Overlay_StopFormRequest, OnFormMenuStop);
        EventSystem.Connect(Signal.Overlay_StartChangeAudio, OnChangeAudioStart);
        EventSystem.Connect(Signal.Overlay_StopChangeAudio, OnChangeAudioStop);
	}
    private bool _firstTimeAudioChange = true;
    private void OnChangeAudioStart(object? sender, EventArgs e) {
        ArgumentNullException.ThrowIfNull(sender);

        VisualElement element = (VisualElement)sender;
        Rect senderBound = element.Bounds;
        AbsoluteLayout.SetLayoutBounds(VolumeSlider, new Rect(
#if WINDOWS
            senderBound.X + 100,
            HolderLayout.Height - senderBound.Height - 10,
            200, 10
#else
            senderBound.X + 5,
            HolderLayout.Height - senderBound.Height - 200,
            10, 200
#endif
            ));
        VolumeSlider.Refresh();

        if (_firstTimeAudioChange) {
            _firstTimeAudioChange = false;
            Dispatcher.DispatchDelayed(
                TimeSpan.FromMilliseconds(Common.Value.TimeSpan.AsyncShortDelay),
                () => {
                    AbsoluteLayout.SetLayoutBounds(VolumeSlider, new Rect(
#if WINDOWS
            senderBound.X + 100,
            HolderLayout.Height - senderBound.Height - 10,
            200, 10
#else
            senderBound.X + 5,
            HolderLayout.Height - senderBound.Height - 200,
            10, 200
#endif
                        ));
                    VolumeSlider.Refresh();
                }
            );
        }

        BackgroundPanel.Opacity = 0;
        this.IsVisible = true;
        VolumeSlider.IsVisible = true;
        OptionMenu.IsVisible = false;
        FormContainer.IsVisible = false;
        HideButton.IsVisible = false;
    }

    private Point _optionTargetPosition = new(0, 0);
    private void OnOptionMenuStart(object? sender, OptionMenuEventArgs e) {
        BackgroundPanel.Opacity = 0;
        OptionMenuHolder.Add(e.Element);

        _optionTargetPosition = e.MouseEvent.GetPosition(this.Parent) ?? new(0, 0);
        Border_SizeChanged(sender, EventArgs.Empty);
        this.IsVisible = true;
        VolumeSlider.IsVisible = true;
        OptionMenu.IsVisible = true;
        FormContainer.IsVisible = false;
        HideButton.IsVisible = false;
    }
    private void OnFormMenuStart(object? sender, FormMenuEventArgs e) {
        BackgroundPanel.Opacity = 0.5;
        FormContainer.Add(e.Element);

        this.IsVisible = true;
        VolumeSlider.IsVisible = false;
        OptionMenu.IsVisible = false;
        FormContainer.IsVisible = true;
        HideButton.IsVisible = true;
    }
    #region Stop&Cancel
    private void OnChangeAudioStop(object? sender, EventArgs e) {
        this.IsVisible = false;
        CleanupUI();
    }
    private void OnOptionMenuStop(object? sender, EventArgs e) {
        this.IsVisible = false;
        CleanupUI();
    }
    private void OnFormMenuStop(object? sender, EventArgs e) {
        this.IsVisible = false;
        CleanupUI();
    }
    private void HideButton_Clicked(object sender, EventArgs e) {
        this.IsVisible = false;
        CleanupUI();
    }
    private void HolderLayoutTapped(object sender, TappedEventArgs e) {
        if (OptionMenu.IsVisible) {
            this.IsVisible = false;
            CleanupUI();
        }
        else if (VolumeSlider.IsVisible) {
            this.IsVisible = false;
            CleanupUI();
        }
    }
    private void CleanupUI() {
        OptionMenuHolder.Children.Clear();
        FormContainer.Children.Clear();
    }
    #endregion
    private void AppOverlaySizeChanged(object sender, EventArgs e) {
        int buttonSize = Setting.ButtonSize;
        FormContainer.WidthRequest = HolderLayout.Width - FormContainer.Margin.Left * 2;
        FormContainer.HeightRequest = HolderLayout.Height - FormContainer.Margin.Top * 2;
        AbsoluteLayout.SetLayoutBounds(HideButton,
            new Rect(HolderLayout.Width - FormContainer.Margin.Left - buttonSize / 2,
                     FormContainer.Margin.Top - buttonSize / 2,
                     buttonSize,
                     buttonSize));
    }
    private void Border_SizeChanged(object? sender, EventArgs e) {
        Rect boundingBox = new(
            _optionTargetPosition.X - OptionMenu.Width,
            _optionTargetPosition.Y,
            AbsoluteLayout.AutoSize,
            AbsoluteLayout.AutoSize
        );
        AbsoluteLayout.SetLayoutBounds(OptionMenu, boundingBox);
    }


}