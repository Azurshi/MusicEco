using SQLiteORM;

namespace MusicEco.Data.Database.Entities;

[Table(
    TableName = null,
    IndexOptions = [],
    TableOptions = ["UNIQUE (Name)"],
    AfterTableOption = ""
)]
internal class PlaylistEntity {
    [PrimaryKey] public DateTime CreationTime { get; init; }
    [DatabaseField] public string Name { get; init; }
    [DatabaseField] public DateTime ModifiedTime { get; init; }
    [DatabaseField] public DateTime LastPlayTime { get; init; }
    public PlaylistEntity((DateTime CreationTime, string Name, DateTime ModifiedTime, DateTime LastPlayTime) t) {
        this.CreationTime = t.CreationTime;
        this.Name = t.Name;
        this.ModifiedTime = t.ModifiedTime;
        this.LastPlayTime = t.LastPlayTime;
    }
}