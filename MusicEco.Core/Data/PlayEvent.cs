using MusicEco.Core.Types;

namespace MusicEco.Core.Data;

public class PlayEvent {
    public DateTime RecordTime { get; init; }
    public Hash256 FileHash { get; init; }
    public TimeSpan PlayedDuration { get; init; }
    public float PlayedRatio { get; init; }
    public PlayEvent(DateTime recordTime, Hash256 fileHash, TimeSpan playedDuration, float playedRatio) {
        this.RecordTime = recordTime;
        this.FileHash = fileHash;
        this.PlayedDuration = playedDuration;
        this.PlayedRatio = playedRatio;
    }
}

