using Blake3;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Repositories;
using System.Text.Json;

namespace MusicEco.Data.Services;

internal class ScanPathService: IScanPathService {
    public event EventHandler? ItemChanged;
    private readonly DictionaryRepository _dictRepo;
    private List<string> _cachedPath;
    private readonly Task _initTask;
    public ScanPathService(DictionaryRepository dictionaryRepository) {
        this._dictRepo = dictionaryRepository;
        this._cachedPath = [];
        this._initTask = LoadPath();
    }
    private async Task LoadPath() {
        var json = await this._dictRepo.GetValue(nameof(ScanPathService));
        if (json != null) {
            _cachedPath = JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
    }
    public async Task<bool> AddPath(string path, object? caller) {
        await _initTask;
        if (!_cachedPath.Contains(path)) {
            this._cachedPath.Add(path);
            try {
                string json = JsonSerializer.Serialize(this._cachedPath);
                await this._dictRepo.SetValue(nameof(ScanPathService), json);
                return true;
            }
            catch {
                await LoadPath();
                return false;
            }
        }
        else {
            return false;
        }
    }

    public async Task<IReadOnlyList<string>> GetPaths() {
        return this._cachedPath;
    }

    public async Task<bool> RemovePath(string path, object? caller = null) {
        await _initTask;
        if (this._cachedPath.Remove(path)) {
            try {
                string json = JsonSerializer.Serialize(this._cachedPath);
                await this._dictRepo.SetValue(nameof(FavouriteService), json);
                return true;
            }
            catch {
                await LoadPath();
                return false;
            }
        }
        else {
            return false;
        }
    }
}
