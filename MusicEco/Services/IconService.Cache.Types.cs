using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Image.Decoder;
using SkiaSharp;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MusicEco.Services;

public partial class IconService {
    private sealed class CustomMemoryCacheOptions {
        public int Capacity;
        public float CompactionPercentage;
        public TimeSpan StrongExpireDelay;
        public TimeSpan WeakExpireDelay;
        public TimeSpan CompactInterval;
        public CustomMemoryCacheOptions(int capacity, float compactionPercentage, TimeSpan strongExpireDelay, TimeSpan weakExpireDelay, TimeSpan compactInterval) {
            this.Capacity = capacity;
            this.CompactionPercentage = compactionPercentage;
            this.StrongExpireDelay = strongExpireDelay;
            this.WeakExpireDelay = weakExpireDelay;
            this.CompactInterval = compactInterval;
        }
    }
    private sealed class WeakMemoryCacheEntry<TItem> where TItem : class {
        public int Size;
        public TimeSpan LastAccess;
        public TimeSpan AddedTime;
        public WeakReference<TItem> ItemRef;
        public WeakMemoryCacheEntry(StrongMemoryCacheEntry<TItem> strongEntry) {
            this.Size = strongEntry.Size;
            this.LastAccess = strongEntry.LastAccess;
            this.AddedTime = strongEntry.AddedTime;
            this.ItemRef = new(strongEntry.Item);
        }
    }
    private sealed class StrongMemoryCacheEntry<TItem> where TItem : class {
        public int Size;
        public TimeSpan LastAccess;
        public TimeSpan AddedTime;
        public TItem Item;
        public StrongMemoryCacheEntry(int size, TimeSpan lastAccess, TimeSpan addedTime, TItem item) {
            this.Size = size;
            this.LastAccess = lastAccess;
            this.AddedTime = addedTime;
            this.Item = item;
        }
        public StrongMemoryCacheEntry(int size, TimeSpan addedTime, TItem item) {
            this.Size = size;
            this.LastAccess = addedTime;
            this.AddedTime = addedTime;
            this.Item = item;
        }
        public StrongMemoryCacheEntry(WeakMemoryCacheEntry<TItem> weakEntry, TItem item) {
            this.Size = weakEntry.Size;
            this.LastAccess = weakEntry.LastAccess;
            this.AddedTime = weakEntry.AddedTime;
            this.Item = item;
        }
    }
    private sealed partial class CustomMemoryCache<TKey>: IDisposable where TKey : notnull {
        private readonly CustomMemoryCacheOptions _options;
        // This hold strong ref to SKImage
        private readonly Dictionary<TKey, SKImage> _referenceHolder;
        // This hold weak ref to IDecoderResult, which act as indirect ref count
        private readonly Dictionary<TKey, WeakMemoryCacheEntry<IDecodeResult>> _weakCache;
        // This is first cache
        private readonly Dictionary<TKey, StrongMemoryCacheEntry<IDecodeResult>> _strongCache;
        private readonly Stopwatch _sw;
        private int _occupied = 0;
        private TimeSpan _lastScan;
        public CustomMemoryCache(CustomMemoryCacheOptions options) {
            this._options = options;
            this._referenceHolder = [];
            this._weakCache = [];
            this._strongCache = [];
            this._sw = Stopwatch.StartNew();
            this._lastScan = TimeSpan.Zero;
        }
        public void Compact() {
            var now = this._sw.Elapsed;
            if (now - this._lastScan < this._options.CompactInterval) {
                return;
            }
            this._lastScan = now;
            // Strong pass
            List<TKey> eligableKeys = [];
            List<ValueTuple<TKey, StrongMemoryCacheEntry<IDecodeResult>>> removedStrongItems = [];
            if (this._occupied > this._options.Capacity) {
                int beforeCompact = this._occupied;
                float targetRemoveSize = this._occupied - (this._options.Capacity * this._options.CompactionPercentage);
                float recordedEvicted = 0;
                var items = this._strongCache.OrderBy(kvp => (kvp.Value.LastAccess, -kvp.Value.Size, kvp.Value.AddedTime)).ToList();
                var expireDelay = this._options.StrongExpireDelay;
                foreach (var (key, entry) in items) {
                    if (now - entry.LastAccess > expireDelay) {
                        eligableKeys.Add(key);
                        targetRemoveSize -= entry.Size;
                        recordedEvicted += entry.Size;
                        if (targetRemoveSize <= 0) {
                            break;
                        }
                    }
                }
                foreach (var key in eligableKeys) {
                    // This always success
                    if (this._strongCache.Remove(key, out var removedEntry)) {
                        removedStrongItems.Add((key, removedEntry));
                        this._occupied -= removedEntry.Size;
                    }
                    else {
                        throw new KeyNotFoundException();
                    }
                }
                eligableKeys.Clear();
                Debug.WriteLine($"Evicted strong: {recordedEvicted} | {this._occupied} / {beforeCompact}");
            }
            // Move to weak
            foreach (var (key, entry) in removedStrongItems) {
                this._weakCache.Add(key, new(entry));
            }
            // Weak pass
            var totalExpireDelay = this._options.WeakExpireDelay + this._options.StrongExpireDelay;
            foreach (var (key, entry) in this._weakCache) {
                if (!entry.ItemRef.TryGetTarget(out _)) {
                    if (now - entry.LastAccess > totalExpireDelay) {
                        eligableKeys.Add(key);
                    }
                }
            }
            int evictedWeak = 0;
            foreach (var key in eligableKeys) {
                // This always success
                if (this._weakCache.Remove(key, out var weakEntry)) {
                    // This always success
                    if (this._referenceHolder.Remove(key, out var image)) {
                        // Dispose here
                        // Evict completely from memory
                        image.Dispose();
                        evictedWeak += weakEntry.Size;
                    }
                    else {
                        throw new KeyNotFoundException();
                    }
                }
                else {
                    throw new KeyNotFoundException();
                }
            }
            if (evictedWeak > 0) {
                Debug.WriteLine($"Evicted weak: {evictedWeak}");
            }
        }

        public void Set(TKey key, IDecodeResult item, int size) {
            if (this._strongCache.TryGetValue(key, out var cachedEntry)) {
                this._occupied -= cachedEntry.Size;
                if (cachedEntry.Item is SkiaDecodeResult skiaResult) {
                    skiaResult.Image.Dispose();
                }
            }
            else if (this._weakCache.Remove(key, out var weakCachedEntry)) {
                if (this._referenceHolder.Remove(key, out var image)) {
                    Debug.WriteLine($"Evicted weak {weakCachedEntry.Size}");
                    image.Dispose();
                }
                else {
                    throw new KeyNotFoundException();
                }
            }
            this._strongCache[key] = new(size, this._sw.Elapsed, item);
            this._referenceHolder[key] = ((SkiaDecodeResult)item).Image;
            this._occupied += size;
        }
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out IDecodeResult item) {
            // Try strong cache first
            if (this._strongCache.TryGetValue(key, out var strongEntry)) {
                item = strongEntry.Item;
                strongEntry.LastAccess = this._sw.Elapsed;
                return true;
            }
            // Then try weak cache
            else if (this._weakCache.Remove(key, out var weakEntry)) {
                if (weakEntry.ItemRef.TryGetTarget(out item)) {
                    weakEntry.LastAccess = this._sw.Elapsed;
                    strongEntry = new(weakEntry, item);
                    this._strongCache.Add(key, strongEntry);
                    this._occupied += strongEntry.Size;
                    return true;
                }
                else {
                    // Recover entry from ref holder
                    if (this._referenceHolder.TryGetValue(key, out var image)) {
                        item = new SkiaDecodeResult(image);
                        weakEntry.LastAccess = this._sw.Elapsed;
                        strongEntry = new(weakEntry, item);
                        this._strongCache.Add(key, strongEntry);
                        this._occupied += strongEntry.Size;
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
            else {
                item = null;
                return false;
            }
        }
        public void Dispose() {
            foreach(var (_, image) in this._referenceHolder) {
                image.Dispose();
            }
        }
    }
}
