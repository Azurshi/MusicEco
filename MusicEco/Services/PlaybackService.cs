using MusicEco.Core.Data;
using MusicEco.Core.Services;
using System.Diagnostics;

namespace MusicEco.Services;

internal partial class PlaybackService: IPlaybackService {
    private readonly IQueueService _queueService;
    private readonly IFileService _fileSerivce;
    private readonly IPlayerController _player;
    private DateTime? _queueKey;
    private static readonly TimeSpan _minimumDelay = TimeSpan.FromSeconds(1);
    private readonly Stopwatch _sw;
    public PlaybackService(IQueueService queueService, IFileService fileService, IPlayerController playerController) {
        this._queueService = queueService;
        this._fileSerivce = fileService;
        this._player = playerController;
        this._player.NextAudioRequested += this.Player_NextAudioRequested;
        this._sw = new();
        this._sw.Start();
        this.StartLastSession();

    }
    private async void StartLastSession() {
        var currentQueue = await this._queueService.GetCurrent();
        this._queueKey = currentQueue?.CreationTime;
        await PlayNew(true, TrackChangeReason.Initialize);
    }
    private async void Player_NextAudioRequested(object? sender, EventArgs e) {
        if (this._queueKey == null) {
            return;
        }
        var queue = await this._queueService.Get(this._queueKey.Value);
        if (queue == null) {
            return;
        }
        queue = queue.Next().WithModifyNow();
        await this._queueService.Update(queue, this);
        await PlayNew(false, TrackChangeReason.AutoNext);
    }

    private async Task PlayNew(bool pause, TrackChangeReason? reason) {
        if (this._queueKey == null) {
            return;
        }
        var queue = await this._queueService.Get(this._queueKey.Value);
        var current = queue?.Current;
        if (current != null) {
            var files = await this._fileSerivce.GetByHash(current.Hash);
            FileEntry? availabeFile = null;
            foreach(var file in files) {
                if (await this._fileSerivce.IsAvailable(file)) {
                    availabeFile = file;
                    break;
                }
            }
            if (availabeFile != null) {
                if (pause) {
                    await this._player.LoadAndPause(availabeFile.Path, availabeFile.Hash, reason);
                } else {
                    await this._player.Play(availabeFile.Path, availabeFile.Hash, reason);
                }
            }
        }
    }
    private async Task PlayQueueInner(string name, List<AudioEntry> audios, AudioEntry current, object? sender, TrackChangeReason? reason) {
        name = name.Trim();
        var now = DateTime.UtcNow;
        var queue = await this._queueService.Get(name);
        if (queue == null) {
            queue = new(now, name, now, now, current, audios);
            await this._queueService.Insert(queue, sender);
        }
        else {
            queue = queue.WithAudios(current, audios).WithModifyNow().WithPlayNow();
            await this._queueService.Update(queue, sender);
        }
        this._queueKey = queue.CreationTime;
        await this._queueService.SetCurrent(queue, sender);
        await PlayNew(false, reason);
    }

    public async Task PlayQueue(string name, List<AudioEntry> audios, AudioEntry current, object? sender) {
        if (this._sw.Elapsed > _minimumDelay) {
            this._sw.Restart();
            await this.PlayQueueInner(name, audios, current, sender, TrackChangeReason.User);
        }
    }
    public async Task PlayQueue(AudioQueue queue, object? sender) {
        if (this._sw.Elapsed > _minimumDelay) {
            this._sw.Restart();
            if (queue.Current == null) {
                if (queue.Audios.Count == 0) {
                    throw new InvalidOperationException();
                }
                queue = queue.WithCurrent(queue.Audios[0]);
            }
            await this.PlayQueueInner(queue.Name, queue.Audios.ToList(), queue.Current!, sender, TrackChangeReason.User);
        }
    }
}
