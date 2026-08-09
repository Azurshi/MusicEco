using MusicEco.Core.Data;
using MusicEco.Data.Database.Entities;
using MusicEco.Data.Database.Relations;
using SQLiteORM;
using System.Linq.Expressions;

namespace MusicEco.Data.Database.Repositories;

internal partial class QueueRepository {
    private static QueueEntity ToEntity(AudioQueue queue, bool isCurrent) {
        return new((queue.CreationTime, queue.Name, queue.ModifiedTime, queue.LastPlayTime, isCurrent));
    }
    private static async Task Insert(SQLiteWriteConnection connection, AudioQueue model, bool isCurrentQueue) {
        QueueEntity queueEntity = ToEntity(model, isCurrentQueue);
        await connection.InsertAsync([queueEntity], false);
        List<QueueAudioRelation> relations = [];
        for (int i = 0; i < model.Audios.Count; i++) {
            var audio = model.Audios[i];
            bool isCurrent;
            if (model.Current == null) {
                isCurrent = false;
            }
            else {
                isCurrent = model.Current.Hash == audio.Hash;
            }
            relations.Add(new((model.CreationTime, audio.Hash, i, isCurrent)));
        }
        await connection.InsertAsync(relations, false);
    }
    private static async Task Delete(SQLiteWriteConnection connection, AudioQueue model) {
        await connection.DeleteAsync(
            "DELETE FROM QueueEntity WHERE CreationTime = ?", model.CreationTime);
        await connection.DeleteAsync(
            "DELETE From QueueAudioRelation WHERE CreationTime = ?", model.CreationTime);
    }
    private static async Task SetCurrent(SQLiteWriteConnection connection, AudioQueue? model) {
        await connection.UpdateAsync($"""
            UPDATE QueueEntity
            SET IsCurrent = CASE
                WHEN CreationTime = ?
                    THEN ?
                    ELSE ?
            END
            """, model?.CreationTime ?? DateTime.MaxValue, true, false);
    }
    public async Task<bool> Insert(AudioQueue model) {
        using (var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                await Insert(connection, model, false);
                await connection.CommitTransactionAsync();
                return true;
            }
            catch {
                await connection.RollbackTransactionAsync();
                return false;
            }
        }
    }
    public async Task<bool> Delete(AudioQueue model) {
        using (var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                await Delete(connection, model);
                await connection.CommitTransactionAsync();
                return true;
            }
            catch {
                await connection.RollbackTransactionAsync();
                return false;
            }
        }
    }
    public async Task<bool> Update(AudioQueue model) {
        using (var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                bool isCurrent = await IsCurrent(connection, model.CreationTime);
                await Delete(connection, model);
                await Insert(connection, model, isCurrent);
                await connection.CommitTransactionAsync();
                return true;
            }
            catch {
                await connection.RollbackTransactionAsync();
                return false;
            }
        }
    }
    public async Task<bool> SetCurrent(AudioQueue? model) {
        using(var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                await SetCurrent(connection, model);
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
