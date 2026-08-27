using MusicEco.Core.Types;
using MusicEco.ViewModels.Overlays;

namespace MusicEco.Views.Overlays;

public partial class AddToQueueOverlay: ContentView, IOverlay {
    public AddToQueueOverlay(AddToQueueOverlayViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
        this._closeAction = new(Close);
    }
    private void Close() {
        this.Closed?.Invoke(this, EventArgs.Empty);
    }
    public event EventHandler? Closed;

    public void ForceClose() {
        this.Closed?.Invoke(this, EventArgs.Empty);
    }
    // Keep the Action alive in the View while ViewModel hold only a weak reference
    private readonly Action _closeAction;
    public async Task Initialize(Hash256 fileHash) {
        var vm = (AddToQueueOverlayViewModel)this.BindingContext;
        await vm.Initialize(fileHash, this._closeAction);
    }
}