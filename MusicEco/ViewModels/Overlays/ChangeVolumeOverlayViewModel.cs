using MusicEco.Core.Services;

namespace MusicEco.ViewModels.Overlays;

public partial class ChangeVolumeOverlayViewModel: BaseOverlayViewModel {
    private readonly IPlayerController _player;
    private const double _volumeEpsilon = 0.01;
    public double Volume {
        get => this._player.GetVolume();
        set {
            double volume = this._player.GetVolume();
            if (value < _volumeEpsilon) {
                value = 0;
            }
            if (value > (1 - _volumeEpsilon)) {
                value = 1.0;
            }
            if (Math.Abs(volume - value) > _volumeEpsilon) {
                float floatValue = (float)value;
                this._player.SetVolume(floatValue);
                OnPropertyChanged();
            }
        }
    }
    public ChangeVolumeOverlayViewModel(ILocalizationService localizationService, IPlayerController playerController) : base(localizationService) {
        this._player = playerController;
    }
}
