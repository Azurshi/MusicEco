using MusicEco.Core.Types;

namespace MusicEco.Core.Data;

public class AudioEntry {
    public Hash256 Hash { get; init; }
    public string Title { get; init; }
    public AudioEntry(Hash256 hash, string title) {
        this.Hash = hash;
        this.Title = title;
    }
}

public class AudioModel {
    public Hash256 Hash { get; }
    public string Title {
        get {
            if (string.IsNullOrEmpty(Metadata.Title)) {
                foreach (var file in Files) {
                    return file.Name;
                }
                return string.Empty;
            }
            else {
                return Metadata.Title;
            }
        }
    }
    public AudioMetadata Metadata { get; }
    public IReadOnlyList<FileEntry> Files { get; }
    public AudioModel(Hash256 hash, AudioMetadata metadata, IReadOnlyList<FileEntry> files) {
        this.Hash = hash;
        this.Metadata = metadata;
        this.Files = files;
    }
}
