using MusicEco.Core.Data;
using MusicEco.Core.Types;
namespace MusicEco.Core.Services;


public class PlayEventChangedEventHandler(ChangeKind kind, DateTime? recordTime): EventArgs {
    public ChangeKind Kind { get; } = kind;
    public DateTime? RecordTime { get; } = recordTime;
}
public interface IPlayEventService {
    public event EventHandler<PlayEventChangedEventHandler> ItemsChanged;
    public abstract Task<bool> Insert(PlayEvent e, object? caller = null);
    public abstract Task<List<PlayEvent>> GetAll(float playedThreshold);
    public abstract Task<List<PlayEvent>> GetByAudio(Hash256 hash, float playedThreshold);
    public abstract Task<PlayEvent?> GetLatest(Hash256 hash, float playedThreshold);
    public abstract Task<Dictionary<Hash256, int>> GetPlaycount(IReadOnlyList<Hash256> hashes, float playedThreshold);
    public abstract Task<int> GetPlaycount(Hash256 hash, float playedThreshold);
    //public abstract Task<bool> UpdatePlayedDuration(PlayEvent e, object? caller = null);
}