using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;
using MusicEco.ViewModels.Pages;
using System.Diagnostics;

namespace MusicEco.ViewModels.Shell;

public partial class ControlBarViewModel: ObservableObject {
    private readonly IAppSetting _setting;
    private readonly IPlayerController _player;
    public AssemblyLocalization L { get; init; }
    protected readonly ILocalizationService _localizationService;
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
    private TimeSpan _playbackDuration = TimeSpan.Zero;
    public string PlaybackPosition => this.Format(this._playbackRatio * this._playbackDuration);
    public string PlaybackDuration => this.Format(this._playbackDuration);
    private string Format(TimeSpan duration) {
        string format = this.L["Format_Time_HourMinuteSecond"];
        return string.Format(format, Math.Floor(duration.TotalHours), duration.Minutes.ToString("D2"), duration.Seconds.ToString("D2"));
    }
    private double _playbackRatio = 0.0;
    public double PlaybackRatio {
        get => this._playbackRatio;
        set {
            if (this._playbackRatio != value) {
                this._playbackRatio = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PlaybackPosition));
            }
        }
    }
    private bool _positionEventUpdateBlocked = false;
    public SyncCommandExtend RatioDragStartedCommand { get; init; }
    public AsyncCommand RatioDragCompletedCommand { get; init; }
    private TimeSpan PerSeekDuration => TimeSpan.FromSeconds(this._setting.Get(15, SettingFields.PerSeekSeconds));
    private const double _volumeEpsilon = 0.01;
    public double Volume {
        get => this._player.GetVolume();
        set {
            double volume = this._player.GetVolume();
            if (value < _volumeEpsilon) {
                value = 0;
            }
            if (value > (1-_volumeEpsilon)) {
                value = 1.0;
            }
            if (Math.Abs(volume - value) > _volumeEpsilon) {
                float floatValue = (float)value;
                this._player.SetVolume(floatValue);
                OnPropertyChanged();
            }
        }
    }
    public ControlBarViewModel(IAppSetting appSetting, IPlayerController playerController, ILocalizationService localizationService) {
        this._setting = appSetting;
        this._localizationService = localizationService;
        this.L = this._localizationService.Get(typeof(BasePageViewModel));
        this._localizationService.LanguageChanged += OnLanguageChanged;
        this.PreviousAudioCommand = new(PreviousAudio);
        this.NextAudioCommand = new(NextAudio);
        this.SeekBackwardCommand = new(SeekBackward, () => this.IsPlaying);
        this.SeekForwardCommand = new(SeekForward, () => this.IsPlaying);
        this.ChangeRepeatCommand = new(ChangeRepeat);
        this.ChangeShuffleCommand = new(ChangeShuffle);
        this.ChangeFavouriteCommand = new(ChangeFavourite);
        this.ToggleVolumeButtonCommand = new(ToggleVolumeButton);
        this.PlayPauseCommand = new(PlayPause);
        this.RatioDragStartedCommand = new(RatioDragStarted, () => !this._positionEventUpdateBlocked);
        this.RatioDragCompletedCommand = new(RatioDragCompleted);
        this._player = playerController;
        this._player.PositionChanged += this.PlayerController_PositionChanged;
        this._player.RepeatingChanged += this.Player_RepeatingChanged;
        this._player.StateChanged += this.Player_StateChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) {
        OnPropertyChanged(nameof(L));
        OnPropertyChanged(nameof(PlaybackPosition));
        OnPropertyChanged(nameof(PlaybackDuration));
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
        if (this._playbackDuration != e.Duration) {
            this._playbackDuration = e.Duration;
            OnPropertyChanged(nameof(PlaybackDuration));
        }
        if (!this._positionEventUpdateBlocked) {
            double ratio = e.Ratio;
            if (Math.Abs(this.PlaybackRatio - ratio) > double.Epsilon) {
                this.PlaybackRatio = ratio;
            }
        }
    }

    private async Task PreviousAudio() {

    }
    private async Task NextAudio() {

    }
    private async Task SeekBackward() {
        var position = this._playbackDuration * this._playbackRatio;
        this._player.Seek(position - this.PerSeekDuration);
    }
    private async Task SeekForward() {
        var position = this._playbackDuration * this._playbackRatio;
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
        }
        else {
            this._player.Resume();
        }
    }
    private void RatioDragStarted() {
        this._positionEventUpdateBlocked = true;
    }
    private async Task RatioDragCompleted() {
        this._player.Seek(this._playbackRatio * this._playbackDuration);
        await Task.Delay(500);
        this._positionEventUpdateBlocked = false;
    }
}
