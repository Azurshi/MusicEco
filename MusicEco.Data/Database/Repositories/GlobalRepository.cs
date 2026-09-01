using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal class GlobalRepository {
    private readonly DatabaseContextAsync _db;
    public GlobalRepository(DatabaseContextAsync dbContext) {
        this._db = dbContext;
    }
    public async Task DeleteAllData() {
        using(var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                var rows = await connection.SelectAsync<string>($"""
                    SELECT 'DROP TABLE IF EXISTS "' || replace(name, '"', '""') || '";'
                    FROM sqlite_master
                    WHERE type = 'table'
                        AND name NOT LIKE 'sqlite_%';
                    """);
                foreach(var row in rows) {
                    await connection.ExecuteAsync(row.Item1);
                }
                await connection.CommitTransactionAsync();
            }
            catch {
                await connection.RollbackTransactionAsync();
            }

        }
    }
}
