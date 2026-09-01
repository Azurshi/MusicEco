using MusicEco.Core.Data;
using MusicEco.Core.Types;
namespace MusicEco.Core.Services;

public class FileChangedEventArgs: EventArgs {
    public ChangeKind Kind { get; init; }
    public HashSet<string> Paths { get; init; }
    public FileChangedEventArgs(ChangeKind kind, HashSet<string> paths) {
        this.Kind = kind;
        this.Paths = paths;
    }
    public FileChangedEventArgs(ChangeKind kind, IReadOnlyList<string> paths) {
        this.Kind = kind;
        this.Paths = [.. paths];
    }
    public FileChangedEventArgs(ChangeKind kind, string path) {
        this.Kind = kind;
        this.Paths = new([path]);
    }
}
public interface IFileService {
    public event EventHandler<FileChangedEventArgs> ItemsChanged;
    public abstract Task<FileEntry?> Get(string path);
    public abstract Task<List<FileEntry>> Query(string path);
    public abstract Task<List<FileEntry>> GetAll();
    public abstract Task<List<FileEntry>> GetByHash(Hash256 hash);
    public abstract Task<bool> UpdateMetadata(string path, Hash256 fileHash, AudioMetadata metadata, object? caller = null);
    public abstract Task<bool> IsAvailable(FileEntry model);
}
