using MusicEco.Core.Data;
using MusicEco.Data.Database.Entities;
using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal partial class PlayEventRepository {
    private static async Task Insert(SQLiteWriteConnection connection, PlayEvent playEvent) {
        PlayEventEntity entity = new((playEvent.RecordTime, playEvent.FileHash, playEvent.PlayedDuration, playEvent.PlayedRatio));
        await connection.InsertAsync([entity], false);
    }
    public async Task<bool> Insert(PlayEvent playEvent) {
        using (var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                await Insert(connection, playEvent);
                await connection.CommitTransactionAsync();
                return true;
            }
            catch {
                await connection.RollbackTransactionAsync();
                return false;
            }
        }
    }
}
