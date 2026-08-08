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

public interface IAudioQueryService {
    public abstract Task<AlbumData?> GetAlbum(string name);
    public abstract Task<List<AlbumData>> QueryAlbum(string nameLike);
}