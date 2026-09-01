using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal class DictionaryRepository {
    private readonly DatabaseContextAsync _db;
    public DictionaryRepository(DatabaseContextAsync dbContext) {
        this._db = dbContext;
    }
    public static async Task<string?> GetValue(SQLiteReadConnection connection, string key) {
        var rows = await connection.SelectAsync<string>(
            "SELECT EntryValue FROM DictionaryEntry WHERE EntryKey = ?", key);
        if (rows.Count > 0) {
            return rows[0].Item1;
        } else {
            return null;
        }
    }
    public static async Task<bool> SetValue(SQLiteWriteConnection connection, string key, string value) {
        var _ = await connection.ExecuteAsync("""
            INSERT INTO DictionaryEntry (EntryKey, EntryValue)
            VALUES (?, ?)
            ON CONFLICT(EntryKey)
            DO UPDATE SET
                EntryKey = excluded.EntryKey,
                EntryValue = excluded.EntryValue
            """, key, value);
        return true;
    }
    public async Task<string?> GetValue(string key) {
        using(var db = await _db.GetReader()) {
            return await GetValue(db.Connection, key);
        }
    }
    public async Task<bool> SetValue(string key, string value) {
        using(var db = await _db.GetWriter()) {
            return await SetValue(db.Connection, key, value);
        }
    }
}
