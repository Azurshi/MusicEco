using MusicEco.Core.Types;
using SQLiteORM;

namespace MusicEco.Data.Database.Entities;

[Table(
    TableName = null,
    IndexOptions = [],
    TableOptions = [],
    AfterTableOption = "WITHOUT ROWID"
)]
internal class IconEntity {
    [PrimaryKey] public Hash256 Hash { get; init; }
    [DatabaseField] public byte[] SmallIcon { get; init; }
    [DatabaseField] public byte[] MediumIcon { get; init; }
    [DatabaseField] public byte[] LargeIcon { get; init; }
    public IconEntity((Hash256 Hash, byte[] SmallIcon, byte[] MediumIcon, byte[] LargeIcon) t) {
        this.Hash = t.Hash;
        this.SmallIcon = t.SmallIcon;
        this.MediumIcon = t.MediumIcon;
        this.LargeIcon = t.LargeIcon;
    }
}
