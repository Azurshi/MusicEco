namespace Domain.AudioPlayer;
public interface IAudioPlayer {
    public abstract event EventHandler? PlaybackEnd;
    public abstract Task Play(string filePath);
    public abstract Task Pause();
    public abstract Task Resume();
    public abstract Task Stop();
    public abstract Task Seek(double miliSeconds);
    public abstract Task SeekTo(double miliSeconds);
    public abstract void SetVolume(float percent);
    public abstract bool IsPlaying { get; }
    public abstract bool IsPaused { get; }
    public abstract bool IsStopped { get; }
    /// <summary>
    /// In miliseconds
    /// </summary>
    public abstract double CurrentTime { get; }
    /// <summary>
    /// In miliseconds
    /// </summary>
    public abstract Task<double> GetTotalTime();
}