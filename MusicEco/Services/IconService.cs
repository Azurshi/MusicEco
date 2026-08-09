using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco.Services;

public partial class IconService: IIconService {
    private readonly IIconDecoder _iconDecoder;
    private readonly IAudioService _audioService;
    private IconCache? _cache;
    private readonly CacheLog _cacheLog;
    private readonly LoadLog _loadLog;
    private readonly Dictionary<IconKey, ValueTuple<Task<byte[]?>, List<CancelSource>>> _tasks;
    private bool _workerInitalized = false;
    private sealed class ManagedBuffer(byte[] buffer) {
        public byte[] Buffer { get; init; } = buffer;
        public bool Busy = false;
        public Memory<byte> GetData(int length) {
            return this.Buffer.AsMemory(0, length);
        }
    }
    private SemaphoreSlim? _bufferLimiter;
    private readonly List<ManagedBuffer> _buffers;
    public IconService(IIconDecoder iconDecoder, IAudioService audioService) {
        this._iconDecoder = iconDecoder;
        this._audioService = audioService;
        this._cacheLog = new();
        this._loadLog = new();
        this._tasks = [];
        this._buffers = [];
    }
    public async Task Setup(int nWorkers, int capacity) {
        if (this._workerInitalized) {
            return;
        }
        this._workerInitalized = true;
        this._cache = new(capacity);
        this._bufferLimiter = new(nWorkers);
        this._iconDecoder.Initialize(nWorkers);
        this._buffers.Capacity = nWorkers;
        this._buffers.Clear();
        for(int i=0; i<nWorkers; i++) {
            var buffer = new byte[Data.Config.LargeIconBufferSize];
            this._buffers.Add(new(buffer));
        }
    }

    public async Task<ImageSource> GetFirstIcon(IReadOnlyList<Hash256> fileHashes, CoverSize size, CancelSource cancelSource) {
        Hash256? iconHash = null;
        foreach(var fileHash in fileHashes) {
            iconHash = await this._audioService.GetCoverHash(fileHash);
            if (iconHash != null) {
                break;
            }
        }
        if (iconHash != null) {
            IconKey key = new(iconHash.Value, size);
            var imageData = await GetIcon(key, cancelSource);
            if (imageData != null) {
                return ImageSource.FromStream(() => new MemoryStream(imageData));
            } else {
                return this._default[size];
            }
        }
        else {
            return this._default[size];
        }
    }
    public async Task<ImageSource> GetIcon(Hash256 fileHash, CoverSize size, CancelSource cancelSource) {
        // This immutable to Metadata change
        var iconHash = await this._audioService.GetCoverHash(fileHash);
        if (iconHash != null) {
            IconKey key = new(iconHash.Value, size);
            var imageData = await GetIcon(key, cancelSource);
            if (imageData != null) {
                return ImageSource.FromStream(() => new MemoryStream(imageData));
            }
            else {
                return this._default[size];
            }
        } else {
            return this._default[size];
        }
    }
    private async Task<byte[]?> GetIcon(IconKey key, CancelSource cancelSource) {
        if (this._cache == null) {
            throw new Exception("Not initialized");
        }
        // ImageSource always decode per UI item
        // So this cache does not worth much
        if (this._cache.TryGet(key, out var image)) {
#if DEBUG
            _cacheLog.Hit();
            _cacheLog.PeriodLog();
            _loadLog.Complete();
            _loadLog.PeriodLog();
#endif
            return image;
        }
        if (this._tasks.TryGetValue(key, out var tuple)) {
            (var task, var sources) = tuple;
            sources.Add(cancelSource); // Modify ref
            if (cancelSource.IsCancelled()) {
                return null;
            } else {
                return await task;
            }
        }
        else {
            List<CancelSource> sources = [cancelSource];
            var task = Loader(key, sources);
            this._tasks[key] = (task, sources);
            var result = await task;
            this._tasks.Remove(key);
            if (cancelSource.IsCancelled()) {
                return null;
            } else {
                return result;
            }
        }
    }
    private static bool IsCancelled(List<CancelSource> sources) {
        foreach (var source in sources) {
            if (!source.IsCancelled()) {
                return false;
            }
        }
        return true;
    }
    private ManagedBuffer AccquireBuffer() {
        foreach(var buffer in this._buffers) {
            if (!buffer.Busy) {
                return buffer;
            }
        }
        throw new Exception("Race condition");
    }
    private async Task<byte[]?> Loader(IconKey key, List<CancelSource> sources) {
        if (this._bufferLimiter == null || this._cache == null) {
            throw new Exception("Not initialized");
        }
        await this._bufferLimiter.WaitAsync();
        if (IsCancelled(sources)) {
#if DEBUG
            _loadLog.Cancel();
            _loadLog.PeriodLog();
#endif
            this._bufferLimiter.Release();
            return null;
        }
        var managedBuffer = AccquireBuffer();
        managedBuffer.Busy = true;
        try {
            int length = await this._audioService.GetCoverData(key.Hash, key.Size, managedBuffer.Buffer);
            if (length <= 0) {
                return null;
            }
            var data = managedBuffer.GetData(length);
            var imageData = await this._iconDecoder.DecodeAsync(data);
            this._cache.Add(key, imageData);
#if DEBUG
            _cacheLog.Miss();
            _cacheLog.PeriodLog();
            _loadLog.Complete();
            _loadLog.PeriodLog();
#endif
            return imageData;
        }
        finally {
            managedBuffer.Busy = false;
            this._bufferLimiter.Release();
        }
    }
}
