using AudioPlayer;
using Domain.AudioPlayer;
using Domain.EventSystem;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEco; 
/// <summary>
/// <see cref="StaticInitializerAttribute"/> priority: 8
/// </summary>
internal static class MusicPlayer {
    public class PlayerAudioEndedEventArgs(long songId) : EventArgs {
        public long SongId { get; set; } = songId;
    }
    public class SongPlaycountChangedEventArgs(long songId, int count) : EventArgs {
        public long SongId { get; set; } = songId;
        public int Count { get; set; } = count;
    }
    public class PlayNextEventArgs(long nextSongId = -1, long currentSongId = -1, bool isAuto = false) : EventArgs {
        public long NextSongId { get; set; } = nextSongId;
        public long CurrentSongId { get; set; } = currentSongId;
        public bool IsAuto { get; set; } = isAuto;
    }
    public class PlayPreviousEventArgs(long previousSongId = -1, long currentSongId = -1) : EventArgs {
        public long PreviousSongId { get; set; } = previousSongId;
        public long CurrentSongId { get; set; } = currentSongId;
    }
    public class PlayerForwardEventArgs(float seconds, float current, float final) : EventArgs {
        public float Seconds { get; set; } = seconds;
        public float Current { get; set; } = current;
        public float Final { get; set; } = final;
    }
    public class PlayerBackwardEventArgs(float seconds, float current, float final) : EventArgs {
        public float Seconds { get; set; } = seconds;
        public float Current { get; set; } = current;
        public float Final { get; set; } = final;
    }
    public class ShuffledEventArgs(long queueId) : EventArgs {
        public long QueueId { get; set; } = queueId;
    }
    private static IAudioPlayer? _player;
    [StaticInitializer(8)]
    public static async Task Initialize() {
#if ANDROID || WINDOWS
        _player = new AudioPlayer.AudioPlayer();
#else
        throw new NotImplementedException("OS not supported");
#endif
        EventSystem.Connect<PlayingQueueChangedEventArgs>(OnQueuePlayingChanged);
        _player.PlaybackEnd += Player_PlaybackEnd;

        Play();
        if (_player.IsPlaying) {
            double totalTime = await _player.GetTotalTime();
            await _player.Seek(GlobalData.PlayerProgress * totalTime);
        }
        SetVolume(GlobalData.PlayerVolume);
        PauseResume();

        while (true) {
            await UpdateProgress();
            await Task.Delay(AppSettingModel._current?.MusicPlayerUpdateDelay ?? 1000);
        }
    }
    private static void OnQueuePlayingChanged(object? sender, PlayingQueueChangedEventArgs e) {
        Play();
    }
    private static void Player_PlaybackEnd(object? sender, EventArgs e) {
        ISongModel? model = GlobalData.CurrenSong;
        long playingSongId = GlobalData.PlayingSongId;
        if (GlobalData.IsRepeat) {
            Play();
        }
        else {
            PlayNext();
        }
        EventSystem.Publish<PlayerAudioEndedEventArgs>(null, new(playingSongId));
        if (model != null) {
            model.PlayCount += 1;
            model.Save();
            EventSystem.Publish<SongPlaycountChangedEventArgs>(null, new(model.Id, model.PlayCount));
        }
    }
    private static async Task UpdateProgress() {
        float progress = 0;
        if (_player != null) {
            progress = (float)(_player.CurrentTime / 1000);
            double totalTime = await _player.GetTotalTime();
            if (totalTime != 0) {
                progress = (float)(_player.CurrentTime / totalTime);
            }
        }
        GlobalData.PlayerProgress = progress;
    }
    #region PublicUse
    public static void Play() {
        IPlaylistModel? queue = GlobalData.CurrentQueue;
        if (queue != null) {
            ISongModel? song = queue.Current;
            if (song != null) {
                Debug.WriteLine($"Play song {song.Title}");
                _player?.Play(song.File.Path);
                GlobalData.IsPlaying = true;
                EventSystem.Publish<PlayingSongChangedEventArgs>(null, new(song.Id));
            }
            else {
                Debug.WriteLine($"Song not found");
            }
        }
        else {
            Debug.WriteLine($"Queue not found {GlobalData.CurrentQueue}");
        }
    }

    public static void PauseResume() {
        if (_player?.IsPlaying ?? throw new Exception("Player not initialized")) {
            _player.Pause();
            GlobalData.IsPlaying = false;
        }
        else if (_player.IsPaused) {
            _player.Resume();
            GlobalData.IsPlaying = true;
        } 
    }
    public static void PlayNext(bool isAuto = false) {
        IPlaylistModel? queue = GlobalData.CurrentQueue;
        if (queue != null) {
            long currentSongId = GlobalData.PlayingSongId;
            if (GlobalData.IsShuffle && queue.IsEndOfList()) {
                queue.Shuffle();
                queue.Current = queue.FirstSong;
                EventSystem.Publish<ShuffledEventArgs>(null, new(queue.Id));
            } else {
                queue.Current = queue.NextSong;
            }
            queue.Save();
            Play();
            EventSystem.Publish<PlayNextEventArgs>(null, new(GlobalData.PlayingQueueId, currentSongId, isAuto));            
        }
    }
    public static void PlayPrevious() {
        IPlaylistModel? queue = GlobalData.CurrentQueue;
        if (queue != null) {
            long currentSongId = GlobalData.PlayingQueueId;
            queue.Current = queue.PreviousSong;
            queue.Save();
            Play();
            EventSystem.Publish<PlayPreviousEventArgs>(null, new(GlobalData.PlayingQueueId, currentSongId));
        }
    }
    public static async Task Forward(float seconds) {
        if (_player != null) {
            float currentTime = (float)_player.CurrentTime / 1000;
            float totalTime = (float)(await _player.GetTotalTime()) / 1000;
            float finalTime = currentTime + seconds;
            if (finalTime > totalTime) {
                finalTime = totalTime;
            }
            await _player.Seek(seconds * 1000);
            EventSystem.Publish<PlayerForwardEventArgs>(null, new(seconds, currentTime, finalTime));
        }
    }
    public static async Task Backward(float seconds) {
        if (_player != null) {
            float currentTime = (float)_player.CurrentTime / 1000;
            float totalTime = (float)(await _player.GetTotalTime()) / 1000;
            float finalTime = currentTime - seconds;
            if (finalTime < 0) {
                finalTime = 0;
            }
            await _player.Seek(-seconds * 1000);
            EventSystem.Publish<PlayerBackwardEventArgs>(null, new(seconds, currentTime, finalTime));
        }
    }
    public static async Task SeekTo(float percent) {
        if (_player != null) {
            double duration = await _player.GetTotalTime();
            await _player.SeekTo(duration * percent);
        }
    }
    public static void SetVolume(float percent) {
        if (_player != null) {
            GlobalData.PlayerVolume = percent;
            _player.SetVolume(percent);
        }
    }
    #endregion
}
