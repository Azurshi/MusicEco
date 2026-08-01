using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Shell;

public partial class ControlBarViewModel: ObservableObject {
    private readonly IAppSetting _setting;
    public bool IsRepeating {
        get => _setting.Get(false);
        set {
            _setting.Set(value);
            OnPropertyChanged();
        }
    }
    public bool IsShuffling {
        get => _setting.Get(false);
        set {
            _setting.Set(value);
            OnPropertyChanged();
        }
    }
    private bool _volumeVisible = false;
    public bool VolumeVisible {
        get => _volumeVisible;
        set {
            _volumeVisible = value;
            OnPropertyChanged();
        }
    }
    public double CurrentVolume {
        get => _setting.Get(0.5);
        set {
            _setting.Set(value);
            OnPropertyChanged();
        }
    }

    public AsyncCommand PreviousAudioCommand { get; }
    public AsyncCommand NextAudioCommand { get; }
    public AsyncCommand SeekBackwardCommand { get; }
    public AsyncCommand SeekForwardCommand { get; }
    public SyncCommand ChangeRepeatCommand { get; }
    public SyncCommand ChangeShuffleCommand { get; }
    public AsyncCommand ChangeFavouriteCommand { get; }
    public SyncCommand ToggleVolumeButtonCommand { get; }
    public AsyncCommand PlayPauseCommand { get; }
    public ControlBarViewModel(IAppSetting appSetting) {
        this._setting = appSetting;
        this.PreviousAudioCommand = new(PreviousAudio);
        this.NextAudioCommand = new(NextAudio);
        this.SeekBackwardCommand = new(SeekBackward);
        this.SeekForwardCommand = new(SeekForward);
        this.ChangeRepeatCommand = new(ChangeRepeat);
        this.ChangeShuffleCommand = new(ChangeShuffle);
        this.ChangeFavouriteCommand = new(ChangeFavourite);
        this.ToggleVolumeButtonCommand = new(ToggleVolumeButton);
        this.PlayPauseCommand = new(PlayPause);
    }
    private async Task PreviousAudio() {

    }
    private async Task NextAudio() {

    }
    private async Task SeekBackward() {

    }
    private async Task SeekForward() {

    }
    private void ChangeRepeat() {
        this.IsRepeating = !this.IsRepeating;
    }
    private void ChangeShuffle() {
        this.IsShuffling = !this.IsShuffling;
    }
    private async Task ChangeFavourite() {

    }
    private void ToggleVolumeButton() {
        this.VolumeVisible = !this.VolumeVisible;
    }
    private async Task PlayPause() {

    }
}
