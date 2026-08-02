using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;

namespace MusicEco.ViewModels.Shell;

public partial class ControlBarViewModel: ObservableObject {
    private readonly IAppSetting _setting;
    private readonly IPlayerController _player;
    public bool IsRepeating {
        get => this._player.IsRepeating;
        set {
            if (this._player.IsRepeating != value) {
                this._player.IsRepeating = value;
            }
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
    public bool IsPlaying { get; private set; }
    public AsyncCommand PreviousAudioCommand { get; }
    public AsyncCommand NextAudioCommand { get; }
    public AsyncCommandExtend SeekBackwardCommand { get; }
    public AsyncCommandExtend SeekForwardCommand { get; }
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
    private TimeSpan PerSeekDuration => TimeSpan.FromSeconds(this._setting.Get(15, SettingFields.PerSeekSeconds));
    public ControlBarViewModel(IAppSetting appSetting, IPlayerController playerController) {
        this._setting = appSetting;
        this.PreviousAudioCommand = new(PreviousAudio);
        this.NextAudioCommand = new(NextAudio);
        this.SeekBackwardCommand = new(SeekBackward, () => this.IsPlaying);
        this.SeekForwardCommand = new(SeekForward, () => this.IsPlaying);
        this.ChangeRepeatCommand = new(ChangeRepeat);
        this.ChangeShuffleCommand = new(ChangeShuffle);
        this.ChangeFavouriteCommand = new(ChangeFavourite);
        this.ToggleVolumeButtonCommand = new(ToggleVolumeButton);
        this.PlayPauseCommand = new(PlayPause);
        this._player = playerController;
        this._player.PositionChanged += this.PlayerController_PositionChanged;
        this._player.RepeatingChanged += this.Player_RepeatingChanged;
        this._player.StateChanged += this.Player_StateChanged;
    }

    private void Player_StateChanged(object? sender, PlayState e) {
        this.IsPlaying = e == PlayState.Playing;
        OnPropertyChanged(nameof(IsPlaying));
        this.SeekBackwardCommand.NotifyCanExecute();
        this.SeekBackwardCommand.NotifyCanExecute();
    }

    private void Player_RepeatingChanged(object? sender, bool e) {
        OnPropertyChanged(nameof(IsRepeating));
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
        var position = this._playbackPosition;
        this._player.Seek(position - this.PerSeekDuration);
    }
    private async Task SeekForward() {
        var position = this._playbackPosition;
        this._player.Seek(position + this.PerSeekDuration);
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
        if (this.IsPlaying) {
            this._player.Pause();
        } else {
            this._player.Resume();
        }
    }
}
