namespace AudioPlayer;

public enum PlaybackState: int {
    /// <summary>
    /// Audio is playing.
    /// </summary>
    Playing,
    /// <summary>
    /// There is no audio yet, or is paused.
    /// </summary>
    Paused,
    /// <summary>
    /// Audio already ended.
    /// </summary>
    End
}
public partial class AudioPlayer: IDisposable {
    private readonly TimeSpan CheckEndEpsilon = TimeSpan.FromSeconds(1);
    public partial void Play(Stream stream);
    public partial void Seek(TimeSpan position);
    public partial void Pause();
    public partial void Resume();
    public partial TimeSpan GetDuration();
    public partial TimeSpan GetPosition();
    public partial TimeSpan GetDecodedPosition();
    public partial float GetVolume();
    public partial void SetVolume(float volume);
    public partial void Dispose();
    private TimeSpan ClampPosition(TimeSpan position) {
        if (position < TimeSpan.Zero) {
            return TimeSpan.Zero;
        }
        else {
            var duration = GetDuration();
            if (position > duration) {
                return duration;
            } else {
                return position;
            }
        }
    }
    private float ClampVolume(float volume) {
        if (volume < 0.0f) {
            return 0.0f;
        } else if (volume > 1.0f) {
            return 1.0f;
        } else {
            return volume;
        }
    }
    public partial PlaybackState GetState();
}
