using MusicEco.Core.Types;
using SQLiteORM;

namespace MusicEco.Data.Database.Entities;

[Table(
    TableName = null,
    IndexOptions = ["Hash"],
    TableOptions = [],
    AfterTableOption = ""
)]
internal class FileEntity {
    [PrimaryKey] public string Path { get; init; }
    [DatabaseField] public Hash256 Hash { get; init; }
    [DatabaseField] public DateTime ModifiedTime { get; init; }
    [DatabaseField] public string Name { get; init; }
    [DatabaseField] public string Extension { get; init; }
    [DatabaseField] public long Size { get; init; }
    public FileEntity((string Path, Hash256 Hash, DateTime ModifiedTime, string Name, string Extension, long Size) t) {
        this.Path = t.Path;
        this.Hash = t.Hash;
        this.ModifiedTime = t.ModifiedTime;
        this.Name = t.Name;
        this.Extension = t.Extension;
        this.Size = t.Size;
    }
}