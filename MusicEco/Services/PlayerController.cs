using MusicEco.Core;
using MusicEco.Core.Services;
using System.Diagnostics;

namespace MusicEco.Services;
public readonly struct AudioTime {
    public readonly TimeSpan Position;
    public readonly TimeSpan Duration;
    public readonly double Ratio {
        get {
            var ratio = Position / Duration;
            ratio = Math.Clamp(ratio, 0.0, 1.0);
            return ratio;
        }
    }
    public AudioTime(TimeSpan position, TimeSpan duration) {
        this.Position = position;
        this.Duration = duration;
    }
}
public partial class PlayerController: IDisposable {
    public event EventHandler<AudioTime>? PositionChanged;
    private readonly AudioPlayer.AudioPlayer _player;
    private FileStream? _stream;
    private bool _disposed = false;
    public PlayerController(IAppSetting setting) {
        this._player = new();
        int targetFps = setting.Get(30, SettingFields.AudioPlayerFPS);
        int delayMs = 1000 / targetFps;
        WorkerLoop(TimeSpan.FromMilliseconds(delayMs)).FireAndForgetAsync();
    }
    private async Task WorkerLoop(TimeSpan delay) {
        Stopwatch sw = new();
        TimeSpan lastUpdate = sw.Elapsed;
        TimeSpan epsilon = TimeSpan.FromMilliseconds(1);
        TimeSpan ceil = TimeSpan.FromMilliseconds(100);
        while (!this._disposed) {
            var elapsed = sw.Elapsed;

            var position = this._player.GetPosition();
            var duration = this._player.GetDuration();
            PositionChanged?.Invoke(this, new(position, duration));

            TimeSpan delta = elapsed - lastUpdate;
            if (delta > ceil) {
                delta = ceil;
            }
            if (delta < epsilon) {
                delta = epsilon;
            }
            lastUpdate = elapsed;
            await Task.Delay(delta);
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
    }
}
