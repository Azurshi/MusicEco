using MusicEco.ViewModels.Overlays;

namespace MusicEco.Views.Overlays;

public partial class CreateNewPlaylistOverlay: ContentView, IOverlay {
    public CreateNewPlaylistOverlay(CreateNewPlaylistOverlayViewModel viewModel) {
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
    public async Task Initialize() {
        var vm = (CreateNewPlaylistOverlayViewModel)this.BindingContext;
        await vm.Initialize(this._closeAction);
    }
}