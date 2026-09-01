using MusicEco.Core.Types;
using SQLiteORM;

namespace MusicEco.Data.Database.Relations;

[Table(
    TableName = null,
    IndexOptions = ["FileHash"],
    TableOptions = [],
    AfterTableOption = ""
)]
internal class PlaylistAudioRelation {
    [PrimaryKey] public DateTime CreationTime { get; init; }
    [PrimaryKey] public Hash256 FileHash { get; init; }
    [PrimaryKey] public int OrderIndex { get; init; }
    public PlaylistAudioRelation((DateTime CreationTime, Hash256 FileHash, int OrderIndex) t) {
        this.CreationTime = t.CreationTime;
        this.FileHash = t.FileHash;
        this.OrderIndex = t.OrderIndex;
    }
}
