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

    public async Task<List<AlbumData>> QueryAlbum(string nameLike) {
        return await this._queryRepo.QueryAlbum(nameLike);
    }
}