namespace MusicEco.Global.AbstractLayers;
public abstract class AudioPlayer {
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
    public abstract double CurrentTime { get; }
    public abstract Task<double> GetTotalTime();
}