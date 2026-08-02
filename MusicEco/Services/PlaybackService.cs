using MusicEco.Core.Data;
using MusicEco.Core.Services;

namespace MusicEco.Services;

internal partial class PlaybackService: IPlaybackService {
    private readonly IQueueService _queueService;
    private readonly IAudioService _audioService;
    private readonly IFileService _fileSerivce;
    private readonly IPlayerController _player;
    private AudioQueue? _playQueue;
    public PlaybackService(IQueueService queueService, IAudioService audioService, IFileService fileService, IPlayerController playerController) {
        this._queueService = queueService;
        this._audioService = audioService;
        this._fileSerivce = fileService;
        this._player = playerController;
    }
    private async Task PlayNewFile(string path) {
        await this._player.Play(path);
    }
    private async Task PlayNew() {
        var current = this._playQueue?.Current;
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
                await PlayNewFile(availabeFile.Path);
            }
        }
    }


    public async Task PlayQueue(string name, List<AudioEntry> audios, AudioEntry current, object? sender) {
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
        this._playQueue = queue;
        await PlayNew();
    }
    public async Task PlayQueue(AudioQueue queue, object? sender) {
        if (queue.Current == null) {
            if (queue.Audios.Count > 0) {
                throw new InvalidOperationException();
            }
            queue = queue.WithCurrent(queue.Audios[0]);
        }
        await this.PlayQueue(queue.Name, queue.Audios.ToList(), queue.Current!, sender);
    }

    public void Dispose() {
        
    }
}
