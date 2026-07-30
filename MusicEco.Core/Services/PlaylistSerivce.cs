using MusicEco.Core.Data;
namespace MusicEco.Core.Services;

public class PlaylistChangedEventArgs(ChangeKind kind, DateTime creationTime): EventArgs {
    public ChangeKind Kind { get; } = kind;
    public DateTime CreationTime { get; } = creationTime;
}

public interface IPlaylistService {
    public event EventHandler<PlaylistChangedEventArgs> ItemsChanged;
    public abstract Task<AudioPlaylist?> Get(DateTime creationTime);
    public abstract Task<bool> Delete(DateTime creationTime, object? caller = null);
    public abstract Task<bool> Update(AudioPlaylist model, object? caller = null);
    public abstract Task<bool> Insert(AudioPlaylist model, object? caller = null);
    public abstract Task<List<AudioPlaylist>> GetAll();
    public abstract Task<List<AudioPlaylist>> Query(string nameLike);
}
