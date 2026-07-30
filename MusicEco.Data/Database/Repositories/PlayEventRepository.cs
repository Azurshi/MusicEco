using MusicEco.Core.Data;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Entities;
using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal class PlayEventRepository {
    private readonly DatabaseContextAsync _db;
    public PlayEventRepository(DatabaseContextAsync dbContext) {
        this._db = dbContext;
    }
    private static PlayEvent ToEvent(PlayEventEntity entity) {
        return new(entity.Time, entity.FileHash, entity.PlayedDuration, entity.PlayedRatio);
    }
    public static async Task<List<PlayEvent>> GetAll(SQLiteReadConnection connection, float playedThreshold) {
        List<PlayEvent> events = [];
        var rows = await connection.SelectAsync<
            DateTime, Hash256, TimeSpan, float>(
            "SELECT * FROM PlayEventEntiy WHERE PlayedRatio >= ?", playedThreshold);
        foreach(var row in rows) {
            PlayEventEntity entity = new(row);
            events.Add(ToEvent(entity));
        }
        return events;
    }
    public static async Task<List<PlayEvent>> GetByAudio(SQLiteReadConnection connection, Hash256 fileHash, float playedThreshold) {
        List<PlayEvent> events = [];
        var rows = await connection.SelectAsync<
            DateTime, Hash256, TimeSpan, float>(
            "SELECT * FROM PlayEventEntiy WHERE FileHash = ? AND PlayedRatio >= ?", fileHash, playedThreshold);
        foreach (var row in rows) {
            PlayEventEntity entity = new(row);
            events.Add(ToEvent(entity));
        }
        return events;
    }
    public static async Task<PlayEvent?> GetLastest(SQLiteReadConnection connection, Hash256 fileHash, float playedThreshold) {
        var rows = await connection.SelectAsync<
            DateTime, Hash256, TimeSpan, float
            >("""
            SELECT * FROM PlayEventEntity
            WHERE FileHash = ? AND PlayedRatio >= ?
                ORDER BY RecordTime ASC
                LIMIT 1
            """, fileHash, playedThreshold);
        foreach (var row in rows) {
            PlayEventEntity entity = new(row);
            return ToEvent(entity);
        }
        return null;
    }
    public static async Task<Dictionary<Hash256, int>> GetPlaycount(SQLiteReadConnection connection, IReadOnlyList<Hash256> fileHashes, float playedThreshold) {
        Dictionary<Hash256, int> counter = [];
        foreach(var batch in fileHashes.Chunk(Config.MaxParameterCount-1)) {
            var placeholder = Config.GetPlaceholder(batch.Length);
            var batchObjs = batch.Cast<object>().ToArray();
            var rows = await connection.SelectAsync<Hash256, int>($"""
                SELECT FileHash, Count(RecordTime) FROM PlayEventEntity
                WHERE PlayedRatio >= ? AND FileHash IN ({placeholder})
                GROUP BY FileHash
                """, playedThreshold, batchObjs);
            foreach (var row in rows) {
                counter[row.Item1] = row.Item2;
            }
        }
        return counter;
    }
    public static async Task<int> GetPlaycount(SQLiteReadConnection connection, Hash256 fileHash, float playedThreshold) {
        var rows = await connection.SelectAsync<int>(
            "SELECT Count(RecordTime) FROM PlayEventEntity WHERE PlayedRatio => ? AND FileHash = ?", playedThreshold, fileHash);
        if (rows.Count > 0) {
            return rows[0].Item1;
        } else {
            return 0;
        }
    }
    public async Task<List<PlayEvent>> GetAll(float playedThreshold) {
        using(var db = await _db.GetReader()) {
            return await GetAll(db.Connection, playedThreshold);
        }
    }
    public async Task<List<PlayEvent>> GetByAudio(Hash256 fileHash, float playedThreshold) {
        using(var db = await _db.GetReader()) {
            return await GetByAudio(db.Connection, fileHash, playedThreshold);
        }
    }
    public async Task<PlayEvent?> GetLastest(Hash256 fileHash, float playedThreshold) {
        using(var db = await _db.GetReader()) {
            return await GetLastest(db.Connection, fileHash, playedThreshold);
        }
    }
    public async Task<Dictionary<Hash256, int>> GetPlaycount(IReadOnlyList<Hash256> fileHashes, float playedThreshold) {
        using(var db = await _db.GetReader()) {
            return await GetPlaycount(fileHashes, playedThreshold);
        }
    }
    public async Task<int> GetPlaycount(Hash256 fileHash, float playedThreshold) {
        using(var db = await _db.GetReader()) {
            return await GetPlaycount(fileHash, playedThreshold);
        }
    }
}
