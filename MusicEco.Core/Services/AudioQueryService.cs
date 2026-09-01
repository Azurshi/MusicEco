using MusicEco.Core.Data;

namespace MusicEco.Core.Services;

public sealed record AlbumData {
    public string Name { get; init; }
    public DateTime QueryTime { get; init; }
    public IReadOnlyList<AudioEntry> Audios { get; init; }
    public AlbumData(string name, DateTime queryTime, IReadOnlyList<AudioEntry> audios) {
        this.Name = name;
        this.QueryTime = queryTime;
        this.Audios = audios;
    }
}
public sealed record PlayCountData {
    public AudioEntry Audio { get; init; }
    public int PlayCount { get; init; }
    public PlayCountData(AudioEntry audio, int playCount) {
        this.Audio = audio;
        this.PlayCount = playCount;
    }
}
public sealed record PlayHistoryData {
    public AudioEntry Audio { get; init; }
    public DateTime LastPlayTime { get; init; }
    public PlayHistoryData(AudioEntry audio, DateTime lastPlayTime) {
        this.Audio = audio;
        this.LastPlayTime = lastPlayTime;
    }
}

public interface IAudioQueryService {
    public abstract Task<AlbumData?> GetAlbum(string name);
    public abstract Task<List<AlbumData>> GetAlbums();
    public abstract Task<List<AudioEntry>> GetNotPlay(float minRatio, string nameLike);
    public abstract Task<List<PlayCountData>> QueryPlayCount(float minRatio, DateTime fromTime, DateTime toTime);
    public abstract Task<List<PlayHistoryData>> GetPlayHistory(float minRatio);
}