using MusicEco.ViewModels.Overlays;

namespace MusicEco.Views.Overlays;

public partial class ChangeVolumeOverlay: ContentView, IOverlay {
    public ChangeVolumeOverlay(ChangeVolumeOverlayViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
    public event EventHandler? Closed;
    public void ForceClose() {
        this.Closed?.Invoke(this, EventArgs.Empty);
    }
}