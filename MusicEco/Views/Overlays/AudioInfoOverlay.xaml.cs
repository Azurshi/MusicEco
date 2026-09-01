using MusicEco.Core.Types;
using MusicEco.ViewModels.Overlays;
using MusicEco.Views.Controls;
using System.Diagnostics;

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

    private void Grid_SizeChanged(object sender, EventArgs e) {
        // Maybe this only work with small number of items
        if (sender is Grid grid) {
            double labelWidth = grid.Width - Utility.GetResource<double>("IconItemSize") - Utility.GetResource<double>("ItemButtonSize");
            foreach(var children in grid.WalkChildren()) {
                if (children is TextLabel label) {
                    //Debug.WriteLine($"Assign: {labelWidth}");
                    label.WidthRequest = labelWidth;
                }
            }
        }
    }

    private void MenuItemButton_Tapped(object sender, EventArgs e) {
        this.Closed?.Invoke(this, EventArgs.Empty);
    }
}