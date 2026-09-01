using MusicEco.Core.Data;
using MusicEco.Core.Types;

namespace MusicEco.Core.Services;

public class QueueChangedEventArgs(ChangeKind kind, DateTime creationTime): EventArgs {
    public ChangeKind Kind { get; } = kind;
    public DateTime CreationTime { get; } = creationTime;
}

public interface IQueueService {
    public event EventHandler<QueueChangedEventArgs> ItemsChanged;
    public event EventHandler CurrentChanged;
    public abstract Task<AudioQueue?> GetCurrent();
    public abstract Task<AudioQueue?> Get(string name);
    public abstract Task<AudioQueue?> Get(DateTime creationTime);
    public abstract Task<bool> Exists(string name);
    public abstract Task<bool> SetCurrent(AudioQueue? current, object? caller = null);
    public abstract Task<bool> Delete(AudioQueue model, object? caller = null);
    public abstract Task<bool> Update(AudioQueue model, object? caller = null);
    public abstract Task<bool> Insert(AudioQueue model, object? caller = null);
    public abstract Task<List<AudioQueue>> GetAll();
    public abstract Task<List<AudioQueue>> Query(string nameLike);
}
