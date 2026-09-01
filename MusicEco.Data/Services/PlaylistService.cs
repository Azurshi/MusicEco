using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Data.Database.Repositories;

namespace MusicEco.Data.Services;

internal class PlaylistService: IPlaylistService {
    public event EventHandler<PlaylistChangedEventArgs>? ItemsChanged;
    private readonly PlaylistRepository _playlistRepo;
    public PlaylistService(PlaylistRepository playlistRepository) {
        this._playlistRepo = playlistRepository;
    }
    public async Task<bool> Delete(DateTime creationTime) {
        var success = await this._playlistRepo.Delete(creationTime);
        if (success) {
            ItemsChanged?.Invoke(this, new(ChangeKind.Removed, creationTime));
            return true;
        }
        else {
            return false;
        }
    }

    public async Task<AudioPlaylist?> Get(DateTime creationTime) {
        return await this._playlistRepo.Get(creationTime);
    }

    public async Task<List<AudioPlaylist>> GetAll() {
        return await this._playlistRepo.GetAll();
    }

    public async Task<bool> Insert(AudioPlaylist model) {
        var success = await this._playlistRepo.Insert(model);
        if (success) {
            ItemsChanged?.Invoke(this, new(ChangeKind.Added, model.CreationTime));
            return true;
        }
        else {
            return false;
        }
    }

    public async Task<List<AudioPlaylist>> Query(string nameLike) {
        return await this._playlistRepo.Query(nameLike);
    }

    public async Task<bool> Update(AudioPlaylist model) {
        var success = await this._playlistRepo.Update(model);
        if (success) {
            ItemsChanged?.Invoke(this, new(ChangeKind.Updated, model.CreationTime));
            return true;
        }
        else {
            return false;
        }
    }
}
