using SQLiteORM;

namespace MusicEco.Data.Database.Entities;

[Table(
    TableName = null,
    IndexOptions = [],
    TableOptions = [],
    AfterTableOption = "WITHOUT ROWID"
)]
internal class DictionaryEntry {
    [PrimaryKey] public string EntryKey { get; set; }
    [JsonField] public string EntryValue { get; set; }
    public DictionaryEntry((string EntryKey, string EntryValue) t) {
        this.EntryKey = t.EntryKey;
        this.EntryValue = t.EntryValue;
    }
}
