using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Data.Database.Repositories;

namespace MusicEco.Data.Services;

internal class AudioQueryService: IAudioQueryService {
    private readonly AudioQueryRepository _queryRepo;
    public AudioQueryService(AudioQueryRepository audioQueryRepository) {
        this._queryRepo = audioQueryRepository;
    }
    public async Task<AlbumData?> GetAlbum(string name) {
        return await this._queryRepo.GetAlbum(name);
    }

    public async Task<List<AudioEntry>> GetNotPlay(float minRatio, string nameLike) {
        return await this._queryRepo.GetNotPlay(minRatio, nameLike);
    }

    public async Task<List<PlayHistoryData>> GetPlayHistory(float minRatio) {
        return await this._queryRepo.GetPlayHistory(minRatio);
    }

    public async Task<List<AlbumData>> QueryAlbum(string nameLike) {
        return await this._queryRepo.QueryAlbum(nameLike);
    }

    public async Task<List<PlayCountData>> QueryPlayCount(float minRatio, DateTime fromTime, DateTime toTime) {
        return await this._queryRepo.GetPlayCount(minRatio, fromTime, toTime);
    }
}