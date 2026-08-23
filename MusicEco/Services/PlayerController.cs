using MusicEco.Core;
using MusicEco.Core.Services;
using System.Diagnostics;
using MusicEco.Core.Types;

#if ANDROID
using MusicEco.Platform;
#endif
namespace MusicEco.Services;
internal partial class PlayerController: IPlayerController {
    private readonly IPlaybackTrackingService _trackingService;
    public event EventHandler<AudioTime>? PositionChanged;
    public event EventHandler? AudioEnded;
    public event EventHandler? NextAudioRequested;
    public event EventHandler<bool>? RepeatingChanged;
    public event EventHandler<PlayState>? StateChanged;
    public event EventHandler<TrackChangedEventArgs>? TrackChanged;

    private readonly IAppSetting _setting;
    private readonly AudioPlayer.AudioPlayer _player;
    private FileStream? _stream;
    private bool _endFlag;
    private PlayState _lastState = PlayState.Stopped;
    private bool _disposed = false;
    private float Volume {
        get => this._setting.Get(0.5f);
        set => this._setting.Set(value);
    }
    public bool IsRepeating {
        get => _setting.Get(false);
        set {
            _setting.Set(value);
            RepeatingChanged?.Invoke(this, value);
        }
    }
    private Hash256 _playing;
    private bool _hasAudio = false;
    private bool IsFirstAudio {
        get => this._setting.Get(true, $"{nameof(PlayerController)}.IsFirstAudio");
        set => this._setting.Set(value, $"{nameof(PlayerController)}.IsFirstAudio");
    }
    public PlayerController(IAppSetting setting, IPlaybackTrackingService playbackTrackingService) {
        this._setting = setting;
        this._trackingService = playbackTrackingService;
        this._player = new();
        int targetFps = this._setting.Get(30, SettingFields.AudioPlayerFPS);
        int delayMs = 1000 / targetFps;
        this._player.SetVolume(Volume);
        this._endFlag = false;
        WorkerLoop(TimeSpan.FromMilliseconds(delayMs)).FireAndForgetAsync();
    }
    private async Task WorkerLoop(TimeSpan delay) {
        Stopwatch sw = Stopwatch.StartNew();
        TimeSpan epsilon = TimeSpan.FromMilliseconds(1);
        while (!this._disposed) {
            sw.Restart();
            var position = this._player.GetPosition();
            var duration = this._player.GetDuration();
            PositionChanged?.Invoke(this, new(position, duration));
            var playerState = this._player.GetState();
            if (this._pauseFlags && playerState == AudioPlayer.PlaybackState.Playing) {
                this._pauseFlags = false;
                this.Pause();
                Debug.WriteLine("PlayerController: Pause on load");
            }
            if (playerState == AudioPlayer.PlaybackState.End) {
                if (!this._endFlag) {
                    this._endFlag = true;
                    await this._trackingService.Record(position, duration);
                    AudioEnded?.Invoke(this, EventArgs.Empty);
                    if (this.IsRepeating) {
                        this._player.Seek(TimeSpan.Zero);
                        this._trackingService.Start(this._playing);
                        this._endFlag = false;
                        this.TrackChanged?.Invoke(this, new(this._playing, TrackChangeReason.Loop));
                    } else {
                        this.NextAudioRequested?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            var state = this.GetState();
            if (this._lastState != state) {
                this._lastState = state;
                StateChanged?.Invoke(this, state);
            }

            var elapsed = sw.Elapsed;
            TimeSpan waitTime = delay - elapsed;
            if (waitTime < epsilon) {
                waitTime = epsilon;
            }
            //Debug.WriteLine($"Log {waitTime.TotalMilliseconds} ms");
            await Task.Delay(waitTime);
        }
    }
    public void Dispose() {
        if (this._disposed) {
            return;
        }
        this._disposed = true;
        this._player.Dispose();
        this._stream?.Close();
    }

    public async Task Play(string path, Hash256 fileHash, TrackChangeReason? forwardedReason) {
        this._playing = fileHash;
        var oldStream = this._stream;
#if WINDOWS
        this._stream = File.OpenRead(path);
#elif ANDROID
        this._stream = UriUtility.OpenFile(Android.Net.Uri.Parse(path)!, 64 * 1024, FileAccess.Read);
#endif
        if (this._stream != null) {
            if (this.IsFirstAudio) {
                this.IsFirstAudio = false;
                this._hasAudio = true;
                this._trackingService.Start(fileHash);
            }
            else if (!this._hasAudio) {
                this._hasAudio = true;
                this._trackingService.Start(fileHash);
            }
            else {
                // Hash audio and not first audio
                await this._trackingService.Record(this._player.GetPosition(), this._player.GetDuration());
                this._trackingService.Start(fileHash);
            }
            this._player.Play(this._stream);
            if (forwardedReason != null) {
                this.TrackChanged?.Invoke(this, new(this._playing, forwardedReason.Value));
            } else {
                Debug.WriteLine("WARNING: Play with no reason");
            }
        }
        oldStream?.Close();
        this._endFlag = false;
    }

    public void Pause() {
        this._player.Pause();
    }

    public void Resume() {
        this._player.Resume();
    }

    public void Seek(TimeSpan position) {
        this._player.Seek(position);
    }

    public void SetVolume(float volume) {
        this.Volume = volume;
        this._player.SetVolume(volume);
    }

    public float GetVolume() {
        return this.Volume;
    }

    public PlayState GetState() {
        return this._player.GetState() switch {
            AudioPlayer.PlaybackState.Playing => PlayState.Playing,
            AudioPlayer.PlaybackState.Paused => PlayState.Stopped,
            AudioPlayer.PlaybackState.End => PlayState.Stopped,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private bool _pauseFlags = false;
    public async Task LoadAndPause(string path, Hash256 fileHash, TrackChangeReason? forwardedReason) {
        this._pauseFlags = true;
        await this.Play(path, fileHash, forwardedReason);
    }
}
