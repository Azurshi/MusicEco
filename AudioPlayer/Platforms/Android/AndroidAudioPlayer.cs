using Android.Media;
using Domain.AudioPlayer;
using Application = Android.App.Application;
using Uri = Android.Net.Uri;

namespace AudioPlayer;

// All the code in this file is only included on Android.
public class AndroidAudioPlayer : IAudioPlayer {
    #region InterfaceImplementation
    public bool IsPlaying => _player.IsPlaying;

    public bool IsPaused => _isPaused;

    public bool IsStopped => _isStopped;

    public double CurrentTime {
        get {
            if (IsPlaying && _firstPlayStarted) {
                return _player.CurrentPosition;
            }
            else if (IsPaused) {
                return _lastPosition;
            }
            else {
                return 0;
            }
        }

    }

    public event EventHandler? PlaybackEnd;

    public Task<double> GetTotalTime() {
        if (IsPaused || (IsPlaying && _firstPlayStarted)) {
            return Task.FromResult((double)_player.Duration);
        }
        else {
            return Task.FromResult(0.0);
        }
    }

    public Task Pause() {
        _isStopped = true;
        _isPaused = false;
        _lastPosition = _player.CurrentPosition;
        _player.Pause();
        return Task.CompletedTask;
    }

    public Task Play(string filePath) {
        _player.Stop();
        _player.Release();
        _player.Dispose();
        _player = new();
        _player.Completion += OnPlayer_Completion;
        _isPaused = false;
        _isStopped = false;
        Uri uri = Android.Net.Uri.Parse(filePath) ?? throw new NullReferenceException();
        _player.SetDataSource(Application.Context, uri);
        _player.Prepare();
        _player.Start();
        _firstPlayStarted = true;
        return Task.CompletedTask;
    }

    public Task Resume() {
        _isPaused = false;
        _isStopped = false;
        _player.Start();
        _player.SeekTo(_lastPosition);
        return Task.CompletedTask;
    }

    public Task Seek(double miliSeconds) {
        int nextPosition = _player.CurrentPosition + (int)miliSeconds;
        if (nextPosition < 0) {
            nextPosition = 0;
        }
        _player.SeekTo(nextPosition);
        return Task.CompletedTask;
    }

    public Task SeekTo(double miliSeconds) {
        _player.SeekTo((int)miliSeconds);
        return Task.CompletedTask;
    }

    public void SetVolume(float percent) {
        _player.SetVolume(percent, percent);
    }

    public Task Stop() {
        _isPaused = false;
        _isStopped = true;
        _player.Stop();
        return Task.CompletedTask;
    }
    #endregion
    private static Uri? GetUriFomItemPath(string filePath) {
        var file = new Java.IO.File(filePath);
        var contentUri = Uri.FromFile(file);
        return contentUri;
    }
    private bool _firstPlayStarted = false;
    private MediaPlayer _player;
    private int _lastPosition;
    private bool _isPaused;
    private bool _isStopped;
    public AndroidAudioPlayer() {
        _player = new();
        _player.Completion += OnPlayer_Completion;
    }

    private void OnPlayer_Completion(object? sender, EventArgs e) {
        PlaybackEnd?.Invoke(this, EventArgs.Empty);
    }
}
