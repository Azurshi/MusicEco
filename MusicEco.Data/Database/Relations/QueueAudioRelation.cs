using MusicEco.Core.Types;
using SQLiteORM;

namespace MusicEco.Data.Database.Relations;

[Table(
    TableName = null,
    IndexOptions = ["FileHash", "IsCurrent"],
    TableOptions = [],
    AfterTableOption = ""
)]
internal class QueueAudioRelation {
    [PrimaryKey] public DateTime CreationTime { get; init; }
    [PrimaryKey] public Hash256 FileHash { get; init; }
    [PrimaryKey] public int OrderIndex { get; init; }
    [DatabaseField] public bool IsCurrent { get; init; }
    public QueueAudioRelation((DateTime CreationTime, Hash256 FileHash, int OrderIndex, bool IsCurrent) t) {
        this.CreationTime = t.CreationTime;
        this.FileHash = t.FileHash;
        this.OrderIndex = t.OrderIndex;
        this.IsCurrent = t.IsCurrent;
    }
}
