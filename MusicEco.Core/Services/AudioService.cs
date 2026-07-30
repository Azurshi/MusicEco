using MusicEco.Core.Data;
using MusicEco.Core.Types;
using System.Numerics;

namespace MusicEco.Core.Services;

public class AudioChangedEventArgs: EventArgs {
    public ChangeKind Kind { get; init; }
    public HashSet<Hash256> Hashes { get; init; }
    public AudioChangedEventArgs(ChangeKind kind, HashSet<Hash256> hashes) {
        this.Kind = kind;
        this.Hashes = hashes;
    }
    public AudioChangedEventArgs(ChangeKind kind, IReadOnlyList<Hash256> hashes) {
        this.Kind = kind;
        this.Hashes = [.. hashes];
    }
    public AudioChangedEventArgs(ChangeKind kind, Hash256 hash) {
        this.Kind = kind;
        this.Hashes = new([hash]);
    }
}

public enum CoverSize {
    Small,
    Medium,
    Large
}

public interface IAudioService {
    public event EventHandler<AudioChangedEventArgs> ItemsChanged;
    public event EventHandler ScanCompleted;
    public event EventHandler ScanStarted;

    public abstract Task<AudioEntry?> GetEntry(Hash256 hash);
    public abstract Task<AudioModel?> Get(Hash256 hash);
    public abstract Task<List<AudioEntry>> GetAllEntry();
    public abstract Task<List<AudioModel>> GetAll();
    public abstract Task<List<AudioEntry>> QueryEntry(string nameLike);
    public abstract Task<List<AudioModel>> Query(string nameLike);
    public abstract Task<Hash256?> GetCoverHash(Hash256 fileHash);
    public abstract Task<int> GetCoverData(Hash256 iconHash, CoverSize size, byte[] buffer);
    //public abstract Task<bool> SetCover(Hash256 iconHash, Memory<byte>? data, Vector2 smallSize, Vector2 mediumSize, Vector2 largeSize); // This is classified as file change
}