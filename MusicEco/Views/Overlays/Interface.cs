namespace MusicEco.Views.Overlays;

public interface IOverlay {
    public event EventHandler? Closing;
    public void ForceClose();
}