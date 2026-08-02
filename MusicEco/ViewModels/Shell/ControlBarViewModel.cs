using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;

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
    private TimeSpan _playbackPosition = TimeSpan.Zero;
    private TimeSpan _playbackDuration = TimeSpan.Zero;
    public string PlaybackPosition => this._playbackPosition.ToString(@"h\:mm\:ss");
    public string PlaybackDuration => this._playbackDuration.ToString(@"h\:mm\:ss");
    public double PlaybackRatio { get; private set; }
    public ControlBarViewModel(IAppSetting appSetting, PlayerController playerController) {
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
        playerController.PositionChanged += this.PlayerController_PositionChanged;
    }

    private void PlayerController_PositionChanged(object? sender, AudioTime e) {
        if(this._playbackPosition != e.Position) {
            this._playbackPosition = e.Position;
            OnPropertyChanged(nameof(PlaybackPosition));
        }
        if (this._playbackDuration != e.Duration) {
            this._playbackDuration = e.Duration;
            OnPropertyChanged(nameof(PlaybackDuration));
        }
        double ratio = e.Ratio;
        if (Math.Abs(this.PlaybackRatio - ratio) > double.Epsilon) {
            this.PlaybackRatio = ratio;
            OnPropertyChanged(nameof(PlaybackRatio));
        }
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
