using Microsoft.Extensions.Caching.Memory;
using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco.Services;

public partial class IconService {
    private sealed record IconKey(Hash256 Hash, CoverSize Size);
    private class IconCache {
        private readonly MemoryCache _cache;
        public IconCache(int capacity) {
            MemoryCacheOptions options = new() {
                ExpirationScanFrequency = TimeSpan.FromSeconds(30),
                SizeLimit = capacity,
                CompactionPercentage = 0.1
            };
            this._cache = new(options);
        }
        public void Add(IconKey key, ImageSource image) {
            MemoryCacheEntryOptions options = new() {
                Size = key.Size switch {
                    CoverSize.Small => 1,
                    CoverSize.Medium => 4,
                    CoverSize.Large => 64,
                    _ => throw new ArgumentOutOfRangeException(nameof(key))
                }
            };
            this._cache.Set(key, image, options);
        }
        public bool TryGet(IconKey key, out ImageSource? image) {
            if (this._cache.TryGetValue(key, out object? result)) {
                if (result is ImageSource imageSource) {
                    image = imageSource;
                    return true;
                }
            }
            image = null;
            return false;
        }
    }
}
