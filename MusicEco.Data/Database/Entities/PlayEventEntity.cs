using MusicEco.Core.Types;
using SQLiteORM;

namespace MusicEco.Data.Database.Entities;

[Table(
    TableName = null,
    IndexOptions = ["FileHash", "PlayedRatio"],
    TableOptions = [],
    AfterTableOption = ""
)]
internal class PlayEventEntity {
    [PrimaryKey] public DateTime Time { get; init; }
    [DatabaseField] public Hash256 FileHash { get; init; }
    [DatabaseField] public TimeSpan PlayedDuration { get; init; }
    [DatabaseField] public float PlayedRatio { get; init; }
    public PlayEventEntity((DateTime Time, Hash256 FileHash, TimeSpan PlayedDuration, float PlayedRatio) t) {
        this.Time = t.Time;
        this.FileHash = t.FileHash;
        this.PlayedDuration = t.PlayedDuration;
        this.PlayedRatio = t.PlayedRatio;
    }
}
