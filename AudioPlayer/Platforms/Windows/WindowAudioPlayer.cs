using Domain.AudioPlayer;
using NAudio.Wave;
using System.Diagnostics;

namespace AudioPlayer;

// All the code in this file is only included on Windows.
public class WindowsAudioPlayer : IAudioPlayer {
    #region InterfaceImplementation
    public bool IsPlaying => _waveOut.PlaybackState == PlaybackState.Playing;

    public bool IsPaused => _waveOut.PlaybackState == PlaybackState.Paused;

    public bool IsStopped => _waveOut.PlaybackState == PlaybackState.Stopped;

    public double CurrentTime {
        get {
            if (_audioFileReader == null) {
                return 0;
            }
            else {
                return _audioFileReader.CurrentTime.TotalMilliseconds;
            }
        }
    }

    public event EventHandler? PlaybackEnd;

    public async Task<double> GetTotalTime() {
        while (_audioFileReader == null) {
            await Task.Delay(Config.ShortWaitDelay);
        }
        return _audioFileReader.TotalTime.TotalMilliseconds;
    }

    public async Task Pause() {
        _lastPosition = (await GetAudioFileReader()).CurrentTime.TotalMilliseconds;
        _waveOut.Pause();
    }


    public async Task Play(string filePath) {
        if (CustomFile.Exists(filePath)) {
            _waveOut.Stop();
            await DisposeAudioFileReader();
            _audioFileReader = new AudioFileReader(filePath);
            _waveOut.Init(_audioFileReader);
            _waveOut.Play();
        }
        else {
            throw new FileNotFoundException($"Audio file not found {filePath}");
        }
    }

    public async Task Resume() {
        _waveOut.Stop();
        (await GetAudioFileReader()).CurrentTime = TimeSpan.FromMilliseconds(_lastPosition);
        _waveOut.Init(_audioFileReader);
        _waveOut.Play();
    }

    public async Task Seek(double miliSeconds) {
        await Pause();
        _waveOut.Stop();
        double newPosition = Math.Clamp(_lastPosition + miliSeconds, 0, await GetTotalTime());
        (await GetAudioFileReader()).CurrentTime = TimeSpan.FromMilliseconds(newPosition);
        _waveOut.Init(_audioFileReader);
        _waveOut.Play();
    }

    public async Task SeekTo(double miliSeconds) {
        AudioFileReader audio = await GetAudioFileReader();
        await Pause();
        _waveOut.Stop();
        double newPosition = Math.Clamp(miliSeconds, 0, await GetTotalTime());
        (await GetAudioFileReader()).CurrentTime = TimeSpan.FromMilliseconds(newPosition);
        _waveOut.Init(_audioFileReader);
        _waveOut.Play();
    }

    public void SetVolume(float percent) {
        _waveOut.Volume = percent;
    }
    public async Task Stop() {
        _waveOut.Stop();
        await DisposeAudioFileReader();
    }
    #endregion
    private readonly IWavePlayer _waveOut;
    private AudioFileReader? _audioFileReader;
    private double _lastPosition = 0;
    public WindowsAudioPlayer() {
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
        int delay = Config.ShortWaitDelay;
        int maxTimeout = Config.ShortTimeout;
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
