using MusicEco.Global.AbstractLayers;
using MusicEco.Global.Attributes;
using MusicEco.Models;
using System.Diagnostics;
using MusicEco.Common.Events;
using MusicEco.Models.Base;
using MusicEco.Common;



#if WINDOWS
using MusicEco.Platforms.Windows;
#else
using MusicEco.Platforms.Android;
#endif

namespace MusicEco.Global;
public static class MusicPlayer {
    #region PersistanceData
    public static int LastPlayedId {
        get => GlobalData.GetValueOrDefault(nameof(MusicPlayer) + nameof(LastPlayedId), -1);
        private set => GlobalData.Set(nameof(MusicPlayer) + nameof(LastPlayedId), value);
    }
    public static int LastPlayedQueue {
        get => GlobalData.GetValueOrDefault(nameof(MusicPlayer) + nameof(LastPlayedQueue), -1);
        private set => GlobalData.Set(nameof(MusicPlayer) + nameof(LastPlayedQueue), value);
    }
    public static bool IsRepeat {
        get => GlobalData.GetValueOrDefault(nameof(MusicPlayer) + nameof(IsRepeat), false);
        private set {
            GlobalData.Set(nameof(MusicPlayer) + nameof(IsRepeat), value);
            RepeatChanged.Invoke(null, new(value));
        }
    }
    public static bool IsShuffle {
        get => GlobalData.GetValueOrDefault(nameof(MusicPlayer) + nameof(IsShuffle), false);
        private set {
            GlobalData.Set(nameof(MusicPlayer) + nameof(IsShuffle), value);
            ShuffleChanged.Invoke(null, new(value));
        }
    }
    public static float Volume {
        get => GlobalData.GetValueOrDefault(nameof(MusicPlayer) + nameof(Volume), 1f);
        private set {
            GlobalData.Set(nameof(MusicPlayer) + nameof(Volume), value);
            VolumeChanged.Invoke(null, new(value));
        }
    }
    public static float Progress {
        get => GlobalData.GetValueOrDefault(nameof(MusicPlayer) + nameof(Progress), 0f);
        private set {
            GlobalData.Set(nameof(MusicPlayer) + nameof(Progress), value);
            ProgressChanged.Invoke(null, new(value));
        }
    }
    public static bool IsPlaying => _player?.IsPlaying ?? false;
    #endregion
    #region InnerSystem
#pragma warning disable CS8618
    private static AudioPlayer _player;
#pragma warning restore CS8618
    private static IDispatcherTimer? _timer;
    [StaticInitializer(1)]
    public static void Initialize() {
#if WINDOWS
        _player = new WindowAudioPlayer();
#else
        _player = new AndroidAudioPlayer();
#endif
        _player.PlaybackEnd += OnPlaybackEnd;
        EventSystem.Connect(Signal.System_Before_UIStart, BeforeUIStart);

        _timer = Dispatcher.GetForCurrentThread()?.CreateTimer();
        if (_timer != null) {
            _timer.Interval = TimeSpan.FromMilliseconds(Common.Value.TimeSpan.MusicPlayerProgressUpdateInterval);
            _timer.Tick += TimerTick;
            _timer.Start();
        }
    }
    private static async void TimerTick(object? sender, EventArgs e) {
        float percent = (float)(_player.CurrentTime / await _player.GetTotalTime());
        if (float.IsNaN(percent)) return;
        Progress = percent;
        EventSystem.Publish<FloatEventArgs>(Signal.Player_Progress_Changed, null, new(percent));
    }
    private static void BeforeUIStart(object? sender, EventArgs e) {
        bool valid = PlayById(LastPlayedId, true).GetAwaiter().GetResult();
        if (valid) {
            PlayPause().GetAwaiter().GetResult();
        }
    }
    private static async void OnPlaybackEnd(object? sender, EventArgs e) {
        SongModel? songModel = SongModel.Get(LastPlayedId);
        if (songModel != null) {
            EventSystem.Publish<IntEventArgs>(Signal.Player_Audio_Ended, null, new(songModel.Id));
            EventSystem.Publish<IntEventArgs>(Signal.Song_Playcount_Changed, null, new(songModel.Id));
            songModel.PlayCount++;
            songModel.Save();
        }
        if (!IsRepeat) {
            await Next();
        } else {
            await _player.Stop();
            await PlayById(LastPlayedId, true);
        }
    }
    private static async Task<bool> PlayById(int songId, bool repeat = false) {
        SongModel? songModel = SongModel.Get(songId);
        if (songModel == null) {
            Debug.WriteLine($"Song model with id {songId} is null");
            return false;
        }
        FileModel? fileModel = songModel.File;
        if (fileModel == null) {
            Debug.WriteLine($"Song model with id {songId} have null file. File id {songModel.FileId}");
            return false;
        }
        string filePath = fileModel.Path;
        if (LastPlayedId != songId || repeat) {
            LastPlayedId = songId;
            EventSystem.Publish<IntEventArgs>(Signal.Player_Song_Changed, null, new(songId));
            await _player.Play(filePath);
        }
        else if (_player.IsPlaying) {
            await _player.Pause();
        }
        else if (_player.IsPaused) {
            await _player.Resume();
        }
        PlayStateChanged.Invoke(null, EventArgs.Empty);
        return true;
    }
    #endregion
    #region PublicAccess
    private static bool _isBusy = false;
    public static readonly WeakEventHandler PlayStateChanged = new();
    public static readonly WeakEventHandler<BoolEventArgs> ShuffleChanged = new();
    public static readonly WeakEventHandler<BoolEventArgs> RepeatChanged = new();
    public static readonly WeakEventHandler<FloatEventArgs> VolumeChanged = new();
    public static readonly WeakEventHandler<FloatEventArgs> ProgressChanged = new();
    public static async Task PlayAudio(int songId, int queueId) {
        while (_isBusy) await Task.Delay(Common.Value.TimeSpan.AsyncBriefDelay);
        _isBusy = true;
        EventSystem.Publish<PlayAudioEventArgs>(Signal.Player_PlayAudio_Requested, null, new(songId, queueId));
        bool valid = await PlayById(songId);
        if (valid) {
            LastPlayedQueue = queueId;
        }
        _isBusy = false;
    }
    public static async Task PlayPause() {
        while (_isBusy) await Task.Delay(Common.Value.TimeSpan.AsyncBriefDelay);
        _isBusy = true;
        if (_player.IsPlaying) {
            await _player.Pause();
            PlayStateChanged.Invoke(null, EventArgs.Empty);
        } else {
            await _player.Resume();
            PlayStateChanged.Invoke(null, EventArgs.Empty);
        }
        _isBusy = false;
    }
    public static async Task Forward(int seconds = 30) {
        while (_isBusy) await Task.Delay(Common.Value.TimeSpan.AsyncBriefDelay);
        _isBusy = true;
        if (_player.IsPlaying) {
            await _player.Seek(seconds * 1000);
        }
        _isBusy = false;
    }
    public static async Task Backward(int seconds = 30) {
        await Forward(-seconds);
    }
    public static void ChangeVolume(float percent) {
        percent = Math.Clamp(percent, 0f, 1f);
        Volume = percent;
        _player.SetVolume(percent);
    }
    public static async Task ChangeProgress(float percent) {
        while (_isBusy) await Task.Delay(Common.Value.TimeSpan.AsyncBriefDelay);
        _isBusy = true;
        if (_player.IsPlaying || _player.IsPaused) {
            await _player.SeekTo(percent * await _player.GetTotalTime());
        }
        _isBusy = false;
    }
    public static async Task Next() {
        PlaylistModel? playlistModel = PlaylistModel.Get(LastPlayedQueue);
        if (playlistModel != null) {
            int id = playlistModel.GetNextId();
            if (id != -1) {
                await PlayById(id);
                playlistModel.CurrentSongId = id;
                playlistModel.Save();
            }
        }
    }
    public static async Task Previous() {
        PlaylistModel? playlistModel = PlaylistModel.Get(LastPlayedQueue);
        if (playlistModel != null) {
            int id = playlistModel.GetPreviousId();
            if (id != -1) {
                await PlayById(id);
                playlistModel.CurrentSongId = id;
                playlistModel.Save();
            }
        }
    }
    public static void SetShuffle(bool value) {
        IsShuffle = value;
    }
    public static void SetRepeat(bool value) {
        IsRepeat = value;
    }
    #endregion
}