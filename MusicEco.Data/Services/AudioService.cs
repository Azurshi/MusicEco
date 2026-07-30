using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Repositories;
using System.Numerics;

namespace MusicEco.Data.Services;

internal class AudioService: IAudioService {
    public event EventHandler<AudioChangedEventArgs>? ItemsChanged;
    public event EventHandler? ScanCompleted;
    public event EventHandler? ScanStarted;
    private readonly AudioRepository _audioRepo;
    private readonly IconRepository _iconRepo;
    public AudioService(AudioRepository audioRepository, IconRepository iconRepository) {
        this._audioRepo = audioRepository;
        this._iconRepo = iconRepository;
    }

    public async Task<AudioModel?> Get(Hash256 hash) {
        var result = await this._audioRepo.Get([hash]);
        if (result.Count > 0) {
            return result[0];
        } else {
            return null;
        }
    }

    public Task<List<AudioModel>> GetAll() {
        return this._audioRepo.GetAll();
    }

    public Task<List<AudioEntry>> GetAllEntry() {
        return this._audioRepo.GetAllEntry();
    }

    public async Task<Hash256?> GetCoverHash(Hash256 fileHash) {
        return await this._iconRepo.GetCoverHash(fileHash);
    }

    public async Task<int> GetCoverData(Hash256 iconHash, CoverSize size, byte[] buffer) {
        return await this._iconRepo.GetCoverData(iconHash, size, buffer);
    }

    public async Task<AudioEntry?> GetEntry(Hash256 hash) {
        var result = await this._audioRepo.GetEntry([hash]);
        if (result.Count > 0) {
            return result[0];
        } else {    
            return null;
        }
    }

    public async Task<List<AudioModel>> Query(string nameLike) {
        return await this._audioRepo.Query(nameLike);
    }

    public async Task<List<AudioEntry>> QueryEntry(string nameLike) {
        return await this._audioRepo.QueryEntry(nameLike);
    }
}
