namespace MusicEco.Views.Overlays;

public interface IOverlay {
    public event EventHandler? Closed;
    public void ForceClose();
}