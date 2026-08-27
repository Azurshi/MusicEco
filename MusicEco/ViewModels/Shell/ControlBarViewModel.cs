using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Pages;
using MusicEco.Views.Overlays;
using System.Numerics;

namespace MusicEco.ViewModels.Shell;

public partial class ControlBarViewModel: ObservableObject {
    private readonly IAppSetting _setting;
    private readonly IServiceProvider _provider;
    private readonly IOverlayService _overlay;
    private readonly IPlayerController _player;
    private readonly IPlaybackService _playback;
    private readonly IQueueService _queueService;
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
    private TimeSpan _playbackDuration = TimeSpan.Zero;
    public string PlaybackPosition => this.Format(this._playbackRatio * this._playbackDuration);
    public string PlaybackDuration => this.Format(this._playbackDuration);
    private string Format(TimeSpan duration) {
        if (duration.TotalHours >= 1) {
            string format = this.L["Format_Time_HourMinuteSecond"];
            return string.Format(format, Math.Floor(duration.TotalHours), duration.Minutes.ToString("D2"), duration.Seconds.ToString("D2"));
        } else {
            string format = this.L["Format_Time_MinuteSecond"];
            return string.Format(format, duration.Minutes.ToString("D2"), duration.Seconds.ToString("D2"));
        }
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
    private TimeSpan PerSeekDuration => TimeSpan.FromSeconds(this._setting.Get(15, SettingFields.PerSeekSeconds));
    public ControlBarViewModel(
        IAppSetting appSetting,
        IServiceProvider serviceProvider,
        IOverlayService overlayService,
        ILocalizationService localizationService,
        IPlayerController playerController, 
        IPlaybackService playbackService, 
        IQueueService queueService
        ) {
        this._setting = appSetting;
        this._localizationService = localizationService;
        this._provider = serviceProvider;
        this._overlay = overlayService;
        this._player = playerController;
        this._playback = playbackService;
        this._queueService = queueService;
        this.L = this._localizationService.Get(typeof(BasePageViewModel));
        this._localizationService.LanguageChanged += OnLanguageChanged;
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
    [RelayCommand]
    private async Task PreviousAudio() {
        var currentQueue = await this._queueService.GetCurrent();
        if (currentQueue != null && currentQueue.Current != null && currentQueue.Audios.Count > 1) {
            currentQueue = currentQueue.Previous().WithModifyNow();
            await this._playback.PlayQueue(currentQueue, this);
        }
    }
    [RelayCommand]
    private async Task NextAudio() {
        var currentQueue = await this._queueService.GetCurrent();
        if (currentQueue != null && currentQueue.Current != null && currentQueue.Audios.Count > 1) {
            currentQueue = currentQueue.Next().WithModifyNow();
            await this._playback.PlayQueue(currentQueue, this);
        }
    }
    private bool CanSeek() {
        return this.IsPlaying;
    }
    [RelayCommand(CanExecute = nameof(CanSeek))]
    private async Task SeekBackward() {
        var position = this._playbackDuration * this._playbackRatio;
        this._player.Seek(position - this.PerSeekDuration);
    }
    [RelayCommand(CanExecute = nameof(CanSeek))]
    private async Task SeekForward() {
        var position = this._playbackDuration * this._playbackRatio;
        this._player.Seek(position + this.PerSeekDuration);
    }
    [RelayCommand]
    private void ChangeRepeat() {
        this.IsRepeating = !this.IsRepeating;
    }
    [RelayCommand]
    private void ChangeShuffle() {
        this.IsShuffling = !this.IsShuffling;
    }
    [RelayCommand]
    private void ToggleVolumeButton() {
        this.VolumeVisible = !this.VolumeVisible;
    }
    [RelayCommand]
    private void ChangeVolume(Vector2? position) {
        if (position == null) {
            return;
        }
        var view = this._provider.GetRequiredService<ChangeVolumeOverlay>();
        this._overlay.ShowFixed(position.Value, view);
    }
    [RelayCommand]
    private async Task PlayPause() {
        if (this.IsPlaying) {
            this._player.Pause();
        }
        else {
            this._player.Resume();
        }
    }
    private bool CanDrag() {
        return !this._positionEventUpdateBlocked;
    }
    [RelayCommand(CanExecute = nameof(CanDrag))]
    private void RatioDragStarted() {
        this._positionEventUpdateBlocked = true;
    }
    [RelayCommand]
    private async Task RatioDragCompleted() {
        this._player.Seek(this._playbackRatio * this._playbackDuration);
        await Task.Delay(500);
        this._positionEventUpdateBlocked = false;
    }
}
