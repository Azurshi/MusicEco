using MusicEco.Global.AbstractLayers;
using NAudio.Wave;
using System.Diagnostics;

namespace MusicEco.Platforms.Windows;
public class WindowAudioPlayer : AudioPlayer {
    #region Base
    public override bool IsPlaying => _waveOut.PlaybackState == PlaybackState.Playing;

    public override bool IsPaused => _waveOut.PlaybackState == PlaybackState.Paused;

    public override bool IsStopped => _waveOut.PlaybackState == PlaybackState.Stopped;

    public override double CurrentTime {
        get {
            if (_audioFileReader == null) {
                return 0;
            } else {
                return _audioFileReader.CurrentTime.TotalMilliseconds;
            }
        }
    }

    public override event EventHandler? PlaybackEnd;
    public override async Task<double> GetTotalTime() {
        while (_audioFileReader == null) {
            await Task.Delay(Common.Value.TimeSpan.AsyncShortDelay);
        }
        return _audioFileReader.TotalTime.TotalMilliseconds;
    }

    public override async Task Pause() {
        _lastPosition = (await GetAudioFileReader()).CurrentTime.TotalMilliseconds;
        _waveOut.Pause();
    }

    public override async Task Play(string filePath) {
        if (Global.AbstractLayers.File.Exists(filePath)) {
            _waveOut.Stop();
            await DisposeAudioFileReader();
            _audioFileReader = new AudioFileReader(filePath);
            _waveOut.Init(_audioFileReader);
            _waveOut.Play();
        } else {
            throw new FileNotFoundException($"Audio file not found {filePath}");
        }
    }

    public override async Task Resume() {
        _waveOut.Stop();
        (await GetAudioFileReader()).CurrentTime = TimeSpan.FromMilliseconds(_lastPosition);
        _waveOut.Init(_audioFileReader);
        _waveOut.Play();
    }

    public override async Task Seek(double miliSeconds) {
        await Pause();
        _waveOut.Stop();
        double newPosition = Math.Clamp(_lastPosition + miliSeconds, 0, await GetTotalTime());
        (await GetAudioFileReader()).CurrentTime = TimeSpan.FromMilliseconds(newPosition);
        _waveOut.Init(_audioFileReader);
        _waveOut.Play();
    }

    public override async Task SeekTo(double miliSeconds) {
        AudioFileReader audio = await GetAudioFileReader();
        await Pause();
        _waveOut.Stop();
        double newPosition = Math.Clamp(miliSeconds, 0, await GetTotalTime());
        (await GetAudioFileReader()).CurrentTime = TimeSpan.FromMilliseconds(newPosition);
        _waveOut.Init(_audioFileReader);
        _waveOut.Play();
    }

    public override void SetVolume(float percent) {
        _waveOut.Volume = percent;
    }

    public override async Task Stop() {
        _waveOut.Stop();
        await DisposeAudioFileReader();
    }
    #endregion
    private readonly IWavePlayer _waveOut;
    private AudioFileReader? _audioFileReader;
    private double _lastPosition = 0;
    public WindowAudioPlayer() {
        _waveOut = new WaveOutEvent();
        _waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
    }
    private void WaveOut_PlaybackStopped(object? sender, StoppedEventArgs e) {
        if (IsStopped) {
            PlaybackEnd?.Invoke(this, EventArgs.Empty);
            Debug.WriteLine("Audio ended");
        }
    }
    private async Task DisposeAudioFileReader() {
        if (_audioFileReader != null) {
            await _audioFileReader.DisposeAsync();
        }
    }
    private async Task<AudioFileReader> GetAudioFileReader() {
        int timeout = 0;
        int delay = Common.Value.TimeSpan.AsyncShortDelay;
        int maxTimeout = Common.Value.TimeSpan.AsyncShorTimeout;
        while (_audioFileReader == null) {
            await Task.Delay(delay);
            timeout += delay;
            if (timeout >= maxTimeout) {
                throw new TimeoutException("Audio file reader timeout");
            }
        }
        return _audioFileReader;
    }

}