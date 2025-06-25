using MusicEco.ViewModels.Components;

namespace MusicEco.Views.Components;

public partial class ControlBar : ContentView, IServiceAccess
{
    private readonly ControlBarModel ViewModel;
    public ControlBar()
	{
		InitializeComponent();
        this.ViewModel = IServiceAccess.Service.GetRequiredService<ControlBarModel>();
        this.BindingContext = ViewModel;
    }
    private async void SetVolumeHolderPosition() {
        while (VolumeChangerHolder.IsVisible) {
            AbsoluteLayout.SetLayoutBounds(VolumeChanger,
                new Rect(VolumeButton.X + VolumeButton.Width / 2, VolumeButton.Y - 100, 10, 100));
            if (!VolumeChanger.IsDragging) {
                VolumeChanger.Percent = ViewModel.PlayerVolume;
            }
            await Task.Delay(100);
        }
    }

    private void VolumeButton_Clicked(object sender, EventArgs e) {
        bool isVisible = !VolumeChangerHolder.IsVisible;
        VolumeChangerHolder.IsVisible = isVisible;
        if (isVisible) {
            Dispatcher.Dispatch(SetVolumeHolderPosition);
        }
    }
}