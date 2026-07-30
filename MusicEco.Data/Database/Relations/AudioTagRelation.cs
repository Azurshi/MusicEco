using MusicEco.Core.Types;
using SQLiteORM;

namespace MusicEco.Data.Database.Relations;

public enum AudioTagType: int {
    Artist,
    ArtistSort,
    AlbumArtist,
    AlbumArtistSort,
    Composer,
    ComposerSort,
    Genre
}

[Table(
    TableName = null,
    IndexOptions = ["FileHash", "Name"],
    TableOptions = [],
    AfterTableOption = ""
)]
public class AudioTagRelation {
    [PrimaryKey] public Hash256 FileHash { get; init; }
    [PrimaryKey] public AudioTagType TagType { get; init; }
    [PrimaryKey] public string Name { get; init; }
    [DatabaseField] public int OrderIndex { get; init; }
    public AudioTagRelation((AudioTagType TagType, Hash256 Hash, string Name, int OrderIndex) t) {
        this.TagType = t.TagType;
        this.FileHash = t.Hash;
        this.Name = t.Name;
        this.OrderIndex = t.OrderIndex;
    }
}
