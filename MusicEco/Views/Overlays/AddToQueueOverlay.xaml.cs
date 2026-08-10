using MusicEco.Core.Types;
using MusicEco.ViewModels.Overlays;

namespace MusicEco.Views.Overlays;

public partial class AddToQueueOverlay: ContentView, IOverlay {
    public AddToQueueOverlay(AddToQueueOverlayViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
    private void Close() {
        Closed?.Invoke(this, EventArgs.Empty);
    }
    public event EventHandler? Closed;

    public void ForceClose() {
        Closed?.Invoke(this, EventArgs.Empty);
    }
    private Action? _closeAction;
    public async Task Initialize(Hash256 fileHash) {
        var vm = (AddToQueueOverlayViewModel)this.BindingContext;
        this._closeAction = new(Close);
        await vm.Initialize(fileHash, this._closeAction);
    }
}