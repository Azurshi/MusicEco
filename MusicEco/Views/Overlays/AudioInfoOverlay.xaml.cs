using MusicEco.Core.Types;
using MusicEco.ViewModels.Overlays;

namespace MusicEco.Views.Overlays;

public partial class AudioInfoOverlay: ContentView, IOverlay {
    public AudioInfoOverlay(AudioInfoOverlayViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
    public event EventHandler? Closed;

    public void ForceClose() {
        Closed?.Invoke(this, EventArgs.Empty);
    }
    public async Task Initialize(Hash256 fileHash) {
        var vm = (AudioInfoOverlayViewModel)this.BindingContext;
        await vm.Initialize(fileHash);
    }

    private void ContentView_SizeChanged(object sender, EventArgs e) {
        this.CoverDisplay.HeightRequest = this.Width / 2;
        var vm = (AudioInfoOverlayViewModel)this.BindingContext;
        vm.FileWidthRequest = this.Width;
    }
}