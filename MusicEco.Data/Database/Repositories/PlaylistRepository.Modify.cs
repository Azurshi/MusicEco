using MusicEco.Core.Data;
using MusicEco.Data.Database.Entities;
using MusicEco.Data.Database.Relations;
using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal partial class PlaylistRepository {
    private static PlaylistEntity ToEntity(AudioPlaylist playist) {
        return new((playist.CreationTime, playist.Name, playist.ModifiedTime, playist.LastPlayTime));
    }
    private static async Task Insert(SQLiteWriteConnection connection, AudioPlaylist model) {
        PlaylistEntity playlistEntity = ToEntity(model);
        await connection.InsertAsync([playlistEntity], false);
        List<PlaylistAudioRelation> relations = [];
        for(int i=0; i<model.Audios.Count; i++) {
            var audio  = model.Audios[i];
            relations.Add(new((model.CreationTime, audio.Hash, i)));
        }
        await connection.InsertAsync(relations, false);
    }
    private static async Task Delete(SQLiteWriteConnection connection, DateTime creationTime) {
        await connection.DeleteAsync(
            "DELETE FROM PlaylistEntity WHERE CreationTime = ?", creationTime);
        await connection.DeleteAsync(
            "DELETE FROM PlaylistAudioRelation WHERE CreationTIme = ?", creationTime);
    }
    public async Task<bool> Insert(AudioPlaylist model) {
        using(var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                await Insert(connection, model);
                await connection.CommitTransactionAsync();
                return true;
            }
            catch {
                await connection.RollbackTransactionAsync();
                return false;
            }
        }
    }
    public async Task<bool> Delete(DateTime creationTime) {
        using(var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                await Delete(connection, creationTime);
                await connection.CommitTransactionAsync();
                return true;
            }
            catch {
                await connection.RollbackTransactionAsync(); ;
                return false;
            }
        }
    }
    public async Task<bool> Update(AudioPlaylist model) {
        using(var db = await this._db.GetWriter()) {
            var connection = db.Connection;
            await connection.BeginTransactionAsync();
            try {
                await Delete(connection, model.CreationTime);
                await Insert(connection, model);
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
