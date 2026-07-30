using MusicEco.Core.Types;

namespace MusicEco.Core.Data;

public class FileEntry {
    public string Path { get; init; }
    public Hash256 Hash { get; init; }
    public DateTime ModifiedTime { get; init; }
    public string Name { get; init; }
    public string Extension { get; init; }
    public long Size { get; init; }
    public FileEntry(string path, Hash256 hash, DateTime modifiedTime, string name, string extension, long size) {
        this.Path = path;
        this.Hash = hash;
        this.ModifiedTime = modifiedTime;
        this.Extension = extension;
        this.Name = name;
        this.Size = size;
    }
    public bool IsSame(FileEntry other) {
        if (Path.Equals(other.Path)) {
            return true;
        }
        else if (Size.Equals(other.Size) && Hash.Equals(other.Hash) && ModifiedTime.Equals(other.ModifiedTime)) {
            return true;
        }
        else {
            return false;
        }
    }
}