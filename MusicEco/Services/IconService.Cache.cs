using Microsoft.Extensions.Caching.Memory;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MusicEco.Services;

public partial class IconService {
    private sealed class CustomMemoryCacheOptions {
        public int Capacity;
        public float CompactionPercentage;
        public TimeSpan MinExpireDelay;
        public TimeSpan ScanInterval;
        public CustomMemoryCacheOptions(int capacity, float compactionPercentage, TimeSpan minExpireDelay, TimeSpan scanInterval) {
            this.Capacity = capacity;
            this.CompactionPercentage = compactionPercentage;
            this.MinExpireDelay = minExpireDelay;
            this.ScanInterval = scanInterval;
        }
    }
    private sealed class CustomMemoryCacheEntry<TItem> {
        public int Size;
        public TimeSpan LastAccess;
        public TimeSpan AddedTime;
        public TItem Item;
        public CustomMemoryCacheEntry(int size, TimeSpan lastAccess, TimeSpan addedTime, TItem item) {
            this.Size = size;
            this.LastAccess = lastAccess;
            this.AddedTime = addedTime;
            this.Item = item;
        }
        public CustomMemoryCacheEntry(int size, TimeSpan addedTime, TItem item) {
            this.Size = size;
            this.LastAccess = addedTime;
            this.AddedTime = addedTime;
            this.Item = item;
        }
    }
    private sealed class CustomMemoryCache<TKey, TItem> where TKey: notnull {
        private readonly CustomMemoryCacheOptions _options;
        private readonly Dictionary<TKey, CustomMemoryCacheEntry<TItem>> _cache;
        private readonly Stopwatch _sw;
        private TimeSpan _lastAutoCompact;
        private int _occupied = 0;
        public CustomMemoryCache(CustomMemoryCacheOptions options) {
            this._options = options;
            this._cache = [];
            this._sw = Stopwatch.StartNew();
        }
        public void Compact() {
            if (this._occupied > this._options.Capacity) {
                int beforeCompact = this._occupied;
                float targetRemoveSize = this._occupied - (this._options.Capacity * this._options.CompactionPercentage);
                float recordedEvicted = 0;
                var items = this._cache.OrderBy(kvp => (kvp.Value.LastAccess, -kvp.Value.Size, kvp.Value.AddedTime)).ToList();
                List<TKey> eligableKeys = [];
                var minExpireDelay = this._options.MinExpireDelay;
                var now = this._sw.Elapsed;
                foreach (var (key, entry) in items) {
                    if (now - entry.LastAccess > minExpireDelay) {
                        eligableKeys.Add(key);
                        targetRemoveSize -= entry.Size;
                        recordedEvicted += entry.Size;
                        if (targetRemoveSize <= 0) {
                            break;
                        }
                    }
                }
                foreach (var key in eligableKeys) {
                    if (this._cache.Remove(key, out var removedEntry)) {
                        this._occupied -= removedEntry.Size;
                    }
                }
                Debug.WriteLine($"Evicted: {recordedEvicted} | {this._occupied} / {beforeCompact}");
            }
        }
        public void AutoCompact() {
            if (this._sw.Elapsed - this._lastAutoCompact > this._options.ScanInterval) {
                this._lastAutoCompact = this._sw.Elapsed;
                this.Compact();
            }
        }
        public void Set(TKey key, TItem item, int size) {
            if (this._cache.TryGetValue(key, out var cachedEntry)) {
                this._occupied -= cachedEntry.Size;
            }
            this._cache[key] = new(size, this._sw.Elapsed, item);
            this._occupied += size;
            this.AutoCompact();
        }
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TItem item) {
            if (this._cache.TryGetValue(key, out var entry)) {
                item = entry.Item;
                entry.LastAccess = this._sw.Elapsed;
                this.AutoCompact();
                return true;
            } else {
                item = default;
                return false;
            }
        }
    }
    private sealed record IconKey(Hash256 Hash, CoverSize Size);
    private class IconCache {
        private readonly CustomMemoryCache<IconKey, byte[]> _cache;
        public IconCache(int capacity) {
            CustomMemoryCacheOptions options = new(capacity, 0.1f, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(5));
            this._cache = new(options);
        }
        public void Add(IconKey key, byte[] image) {
            int size = key.Size switch {
                CoverSize.Small => 1,
                CoverSize.Medium => 4,
                CoverSize.Large => 64,
                _ => throw new ArgumentOutOfRangeException(nameof(key))
            };
            this._cache.Set(key, image, size);
        }
        public bool TryGet(IconKey key, [MaybeNullWhen(false)] out byte[] image) {
            if (this._cache.TryGetValue(key, out var imageSource)) {
                image = imageSource;
                return true;
            }
            image = null;
            return false;
        }
    }
}
