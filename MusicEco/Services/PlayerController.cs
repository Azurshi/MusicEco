using MusicEco.Core;
using MusicEco.Core.Services;
using System.Diagnostics;

namespace MusicEco.Services;
internal partial class PlayerController: IPlayerController {
    public event EventHandler<AudioTime>? PositionChanged;
    public event EventHandler? AudioEnded;
    public event EventHandler<bool>? RepeatingChanged;
    public event EventHandler<PlayState>? StateChanged;

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
    public PlayerController(IAppSetting setting) {
        this._setting = setting;
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
            if (this._player.GetState() == AudioPlayer.PlaybackState.End) {
                if (!this._endFlag) {
                    this._endFlag = true;
                    AudioEnded?.Invoke(this, EventArgs.Empty);
                    if (this.IsRepeating) {
                        this._player.Seek(TimeSpan.Zero);
                        this._endFlag = false;
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

    public async Task Play(string path) {
        var oldStream = this._stream;
#if WINDOWS
        this._stream = File.OpenRead(path);
#endif
        if (this._stream != null) {
            this._player.Play(this._stream);
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
}
