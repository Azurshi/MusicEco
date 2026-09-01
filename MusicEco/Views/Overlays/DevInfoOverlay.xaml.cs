namespace MusicEco.Views.Overlays;

public partial class DevInfoOverlay: ContentView, IOverlay {
    public DevInfoOverlay() {
        InitializeComponent();
    }

    public event EventHandler? Closed;
    public void Initialize(string text) {
        this.OutputLabel.Text = text;
    }
    public void ForceClose() {
        this.Closed?.Invoke(this, EventArgs.Empty);
    }
}
