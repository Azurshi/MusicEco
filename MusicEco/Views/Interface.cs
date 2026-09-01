namespace MusicEco.Views;

public interface IAnimatedView {
    public bool IsAnimationEnabled { get; set; }
    public void OnEnterViewport();
    public void OnExitViewport();
}