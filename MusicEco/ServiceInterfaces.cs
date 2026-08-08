using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco;

public interface IPageRouteRegistry {
    public Type GetPageType(PageRoute route);
}
public interface IPageResolver {
    public ContentView GetPage(PageRoute route);
}

public class CancelSource {
    public WeakReference<object> SenderReference { get; }
    public CancellationToken Token { get; }
    public CancelSource(object sender, CancellationToken token) {
        SenderReference = new(sender);
        Token = token;
    }
    public bool IsCancelled() {
        if (Token.IsCancellationRequested || !SenderReference.TryGetTarget(out var _)) {
            return true;
        }
        else {
            return false;
        }
    }
}

public interface IIconService {
    public Task InitializeDefault(IServiceProvider provider);
    public Task Setup(int nWorkers, int capacity);
    public Task<ImageSource> GetIcon(Hash256 fileHash, CoverSize size, CancelSource cancelSource);
    public Task<ImageSource> GetFirstIcon(IReadOnlyList<Hash256> fileHashes, CoverSize size, CancelSource cancelSource);
}

public interface IPlaybackService: IDisposable {
    public Task PlayQueue(string name, List<AudioEntry> audios, AudioEntry current, object? sender);
    public Task PlayQueue(AudioQueue queue, object? sender);
}

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

public enum PlayState {
    Playing,
    Stopped
}

public interface IPlayerController: IDisposable {
    public event EventHandler<AudioTime>? PositionChanged;
    public event EventHandler? AudioEnded;
    public event EventHandler<bool>? RepeatingChanged;
    public event EventHandler<PlayState>? StateChanged;
    public bool IsRepeating { get; set; }
    public Task Play(string path);
    public void Pause();
    public void Resume();
    public void Seek(TimeSpan position);
    public void SetVolume(float volume);
    public float GetVolume();
    public PlayState GetState();
}