namespace MusicEco.Core.Data;

public class AudioPlaylist {
    public string Name { get; init; }
    public DateTime CreationTime { get; init; }
    public DateTime ModifiedTime { get; init; }
    public DateTime LastPlayTime { get; init; }
    public IReadOnlyList<AudioEntry> Audios { get; init; }
    public AudioPlaylist(string name, DateTime creationTime, DateTime modifiedTime, DateTime lastPlayTime, IReadOnlyList<AudioEntry> audios) {
        this.Name = name;
        this.CreationTime = creationTime;
        this.ModifiedTime = modifiedTime;
        this.LastPlayTime = lastPlayTime;
        this.Audios = audios;
    }
    public AudioPlaylist WithName(string name) {
        return new(name, CreationTime, ModifiedTime, LastPlayTime, Audios);
    }
    public AudioPlaylist WithModifyNow() {
        return new(Name, CreationTime, DateTime.Now, LastPlayTime, Audios);
    }
    public AudioPlaylist WithPlayNow() {
        return new(Name, CreationTime, ModifiedTime, DateTime.Now, Audios);
    }
    public AudioPlaylist WithAudios(IReadOnlyList<AudioEntry> audios) {
        return new(Name, CreationTime, ModifiedTime, LastPlayTime, audios);
    }
}
