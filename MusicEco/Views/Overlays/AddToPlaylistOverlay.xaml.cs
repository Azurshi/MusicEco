using MusicEco.Core.Types;
using MusicEco.ViewModels.Overlays;

namespace MusicEco.Views.Overlays;

public partial class AddToPlaylistOverlay: ContentView, IOverlay {
    public AddToPlaylistOverlay(AddToPlaylistOverlayViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
        this._closeAction = new(Close);
    }
    private void Close() {
        Closed?.Invoke(this, EventArgs.Empty);
    }
    public event EventHandler? Closed;

    public void ForceClose() {
        Closed?.Invoke(this, EventArgs.Empty);
    }
    // Keep the Action alive in the View while ViewModel hold only a weak reference
    private readonly Action _closeAction;
    public async Task Initialize(Hash256 fileHash) {
        var vm = (AddToPlaylistOverlayViewModel)this.BindingContext;
        await vm.Initialize(fileHash, this._closeAction);
    }
}