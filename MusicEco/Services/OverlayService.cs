using MusicEco.Views.Shell;

namespace MusicEco.Services;

public class OverlayService {
    private readonly AppOverlay _view;
    public OverlayService(AppOverlay overlayView) {
        this._view = overlayView;
    }
}
