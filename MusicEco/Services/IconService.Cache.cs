using Microsoft.Extensions.Caching.Memory;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MusicEco.Services;

public partial class IconService {
    private readonly struct IconKey: IEquatable<IconKey> {
        public readonly Hash256 Hash;
        public readonly CoverSize Size;
        public IconKey(Hash256 hash, CoverSize size) {
            this.Hash = hash;
            this.Size = size;
        }

        public bool Equals(IconKey other) {
            return this.Size == other.Size && this.Hash == other.Hash;
        }

        public override bool Equals(object? obj) {
            if (obj is IconKey other) {
                return this.Size == other.Size && this.Hash == other.Hash;
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() {
            return this.Hash.GetHashCode() + (int)this.Size;
        }
    }
    private partial class IconCache: IDisposable {
        private readonly CustomMemoryCache<IconKey> _cache;
        public IconCache(int capacity) {
            CustomMemoryCacheOptions options = new(
                capacity, 
                0.1f, 
                TimeSpan.FromSeconds(5), 
                TimeSpan.FromSeconds(60), 
                TimeSpan.FromSeconds(5));
            this._cache = new(options);
        }
        public void Add(IconKey key, IDecodeResult image) {
            int size = key.Size switch {
                CoverSize.Small => 1,
                CoverSize.Medium => 4,
                CoverSize.Large => 64,
                _ => throw new ArgumentOutOfRangeException(nameof(key))
            };
            this._cache.Set(key, image, size);
        }
        public bool TryGet(IconKey key, [MaybeNullWhen(false)] out IDecodeResult decodeResult) {
            if (this._cache.TryGetValue(key, out var imageSource)) {
                decodeResult = imageSource;
                return true;
            }
            decodeResult = default;
            return false;
        }
        public void Compact() {
            this._cache.Compact();
        }

        public void Dispose() {
            this._cache.Dispose();
        }
    }
}
