using MusicEco.Core.Data;
using MusicEco.Core.Types;

namespace MusicEco.Core.Services;

public interface IFavouriteService {
    public event EventHandler ItemsChanged;
    public Task<bool> IsFavourite(Hash256 hash);
    public Task<bool> AddFavourite(Hash256 hash, object? caller = null);
    public Task<bool> RemoveFavourite(Hash256 hash, object? caller = null);
    public Task<List<AudioEntry>> GetFavourites();
}
