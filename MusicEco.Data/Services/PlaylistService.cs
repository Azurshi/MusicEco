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
    public Task<bool> Delete(DateTime creationTime, object? caller = null) {
        throw new NotImplementedException();
    }

    public async Task<AudioPlaylist?> Get(DateTime creationTime) {
        return await this._playlistRepo.Get(creationTime);
    }

    public async Task<List<AudioPlaylist>> GetAll() {
        return await this._playlistRepo.GetAll();
    }

    public Task<bool> Insert(AudioPlaylist model, object? caller = null) {
        throw new NotImplementedException();
    }

    public async Task<List<AudioPlaylist>> Query(string nameLike) {
        return await this._playlistRepo.Query(nameLike);
    }

    public Task<bool> Update(AudioPlaylist model, object? caller = null) {
        throw new NotImplementedException();
    }
}
