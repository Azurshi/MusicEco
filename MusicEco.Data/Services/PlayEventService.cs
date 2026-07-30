using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Repositories;

namespace MusicEco.Data.Services;

internal class PlayEventService: IPlayEventService {
    public event EventHandler<PlayEventChangedEventHandler>? ItemsChanged;
    private readonly PlayEventRepository _eventRepo;
    public PlayEventService(PlayEventRepository playEventRepository) {
        this._eventRepo = playEventRepository;
    }
    public async Task<List<PlayEvent>> GetAll(float playedThreshold) {
        return await this._eventRepo.GetAll(playedThreshold);
    }

    public async Task<List<PlayEvent>> GetByAudio(Hash256 hash, float playedThreshold) {
        return await this._eventRepo.GetByAudio(hash, playedThreshold);
    }

    public async Task<PlayEvent?> GetLatest(Hash256 hash, float playedThreshold) {
        return await this._eventRepo.GetLastest(hash, playedThreshold);
    }

    public async Task<Dictionary<Hash256, int>> GetPlaycount(IReadOnlyList<Hash256> hashes, float playedThreshold) {
        return await this._eventRepo.GetPlaycount(hashes, playedThreshold);
    }

    public async Task<int> GetPlaycount(Hash256 hash, float playedThreshold) {
        return await this._eventRepo.GetPlaycount(hash, playedThreshold);
    }

    public Task<bool> Insert(PlayEvent e, object? caller = null) {
        throw new NotImplementedException();
    }

    public Task<bool> UpdatePlayedDuration(PlayEvent e, object? caller = null) {
        throw new NotImplementedException();
    }


}
