using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Repositories;
using System.Text.Json;

namespace MusicEco.Data.Services;

internal class FavouriteService: IFavouriteService {
    public event EventHandler? ItemsChanged;
    private readonly DictionaryRepository _dictRepo;
    private readonly AudioRepository _audioRepo;
    private List<Hash256> _cachedHash;
    private readonly Task _initTask;
    public FavouriteService(DictionaryRepository dictionaryRepository, AudioRepository audioRepository) {
        this._dictRepo = dictionaryRepository;
        this._audioRepo = audioRepository;
        this._cachedHash = [];
        this._initTask = LoadHash();
    }
    public async Task<bool> AddFavourite(Hash256 hash, object? caller = null) {
        await _initTask;
        if (!_cachedHash.Contains(hash)) {
            this._cachedHash.Add(hash);
            try {
                string json = JsonSerializer.Serialize(this._cachedHash);
                await this._dictRepo.SetValue(nameof(FavouriteService), json);
                return true;
            }
            catch {
                await LoadHash();
                return false;
            }
        } else {
            return false;
        }
    }
    private async Task LoadHash() {
        var json = await this._dictRepo.GetValue(nameof(FavouriteService));
        if (json != null) {
            _cachedHash = JsonSerializer.Deserialize<List<Hash256>>(json) ?? [];
        }
    }
    public async Task<List<AudioEntry>> GetFavourites() {
        await _initTask;
        return await this._audioRepo.GetEntry(_cachedHash);
    }

    public async Task<bool> IsFavourite(Hash256 hash) {
        await _initTask;
        return _cachedHash.Contains(hash);
    }

    public async Task<bool> RemoveFavourite(Hash256 hash, object? caller = null) {
        await _initTask;
        if (_cachedHash.Remove(hash)) {
            try {
                string json = JsonSerializer.Serialize(this._cachedHash);
                await this._dictRepo.SetValue(nameof(FavouriteService), json);
                return true;
            }
            catch {
                await LoadHash();
                return false;
            }
        } else {
            return false;
        }
    }
}
