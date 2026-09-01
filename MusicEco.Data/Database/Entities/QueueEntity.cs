using SQLiteORM;

namespace MusicEco.Data.Database.Entities;

[Table(
    TableName = null,
    IndexOptions = ["IsCurrent"],
    TableOptions = ["UNIQUE (Name)"],
    AfterTableOption = ""
)]
internal class QueueEntity {
    [PrimaryKey] public DateTime CreationTime { get; init; }
    [DatabaseField] public string Name { get; init; }
    [DatabaseField] public DateTime ModifiedTime { get; init; }
    [DatabaseField] public DateTime LastPlayTime { get; init; }
    [DatabaseField] public bool IsCurrent { get; init; }
    public QueueEntity((DateTime CreationTime, string Name, DateTime ModifiedTime, DateTime LastPlayTime, bool IsCurrent) t) {
        this.CreationTime = t.CreationTime;
        this.Name = t.Name;
        this.ModifiedTime = t.ModifiedTime;
        this.LastPlayTime = t.LastPlayTime;
        this.IsCurrent = t.IsCurrent;
    }
}