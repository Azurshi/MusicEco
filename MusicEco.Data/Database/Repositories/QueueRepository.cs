using MusicEco.Core.Data;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Entities;
using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal partial class QueueRepository {
    private readonly DatabaseContextAsync _db;
    public QueueRepository(DatabaseContextAsync dbContext) {
        this._db = dbContext;
    }
    public static async Task<List<AudioQueue>> Get(SQLiteReadConnection connection, List<DateTime> creationTimes) {
        List<AudioQueue> queues = [];
        var creationTimeObjs = creationTimes.Cast<object>().ToArray();
        var rows = await connection.SelectAsync<
            DateTime, string, DateTime, DateTime, bool>($"""
            SELECT * FROM QueueEntity
            WHERE CreationTime IN ({Config.GetPlaceholder(creationTimeObjs.Length)})
            """, creationTimeObjs);
        List<ValueTuple<DateTime, Hash256, bool>> hashRows = [];
        foreach (var batchObjs in creationTimeObjs.Chunk(Config.MaxParameterCount)) {
            var placeholder = Config.GetPlaceholder(batchObjs.Length);
            var batchResult = await connection.SelectAsync<DateTime, Hash256, bool>($"""
                SELECT CreationTime, FileHash, IsCurrent
                FROM QueueAudioRelation
                WHERE CreationTime IN ({placeholder})
                ORDER BY OrderIndex
                """, batchObjs);
            hashRows.AddRange(batchResult);
        }
        var hashes = hashRows.Select(r => r.Item2).Distinct().ToList();
        List<AudioEntry> audios = await AudioRepository.GetEntry(connection, hashes);
        Dictionary<Hash256, AudioEntry> audioMap = audios.ToDictionary(a => a.Hash);
        Dictionary<DateTime, List<ValueTuple<AudioEntry, bool>>> entriesDataMap = [];
        foreach (var (creationTime, hash, isCurrent) in hashRows) {
            if (!entriesDataMap.TryGetValue(creationTime, out var entriesData)) {
                entriesData = [];
                entriesDataMap[creationTime] = entriesData;
            }
            entriesData.Add((audioMap[hash], isCurrent));
        }
        foreach (var row in rows) {
            QueueEntity entity = new(row);
            var entriesData = entriesDataMap[entity.CreationTime];
            List<AudioEntry> entries = [];
            AudioEntry? current = null;
            foreach(var (entry, isCurrent) in entriesData) {
                entries.Add(entry);
                if (isCurrent) {
                    current = entry;
                }
            }
            AudioQueue queue = new(entity.CreationTime, entity.Name, entity.ModifiedTime, entity.LastPlayTime, current, entries);
            queues.Add(queue);
        }
        return queues;
    }
    public static async Task<AudioQueue?> GetCurrent(SQLiteReadConnection connection) {
        var rows = await connection.SelectAsync<DateTime>(
            "SELECT CreationTime FROM QueueEntity WHERE IsCurrent = ?", true);
        if (rows.Count > 0) {
            return await Get(connection, rows[0].Item1);
        } else {
            return null;
        }
    }
    public static async Task<bool> IsCurrent(SQLiteReadConnection connection, DateTime creationTime) {
        var rows = await connection.SelectAsync<bool>($"""
            SELECT EXISTS (
                SELECT 1
                FROM QueueEntity 
                WHERE CreationTime = ? AND IsCurrent = ?
            )
            """, creationTime, true);
        return rows.First().Item1;
    }
    public static async Task<AudioQueue?> Get(SQLiteReadConnection connection, string name) {
        var rows = await connection.SelectAsync<DateTime>(
            "SELECT CreationTime FROM QueueEntity WHERE Name = ?", name);
        if (rows.Count > 0) {
            return await Get(connection, rows[0].Item1);
        } else {
            return null;
        }
    }
    public static async Task<AudioQueue?> Get(SQLiteReadConnection connection, DateTime creationTime) {
        var queues = await Get(connection, [creationTime]);
        if (queues.Count > 0) {
            return queues[0];
        } else {
            return null;
        }
    }
    public static async Task<bool> Exists(SQLiteReadConnection connectionn, string name) {
        var rows = await connectionn.SelectAsync<string>(
            "SELECT Name FROM QueueEntity WHERE Name = ?", name);
        if (rows.Count > 0) {
            return true;
        } else {
            return false;
        }
    }
    public static async Task<List<AudioQueue>> GetAll(SQLiteReadConnection connection) {
        var rows = await connection.SelectAsync<DateTime>(
            "SELECT CreationTime FROM QueueEntity");
        return await Get(connection, rows.Select(r => r.Item1).ToList());
    }
    public static async Task<List<AudioQueue>> Query(SQLiteReadConnection connection, string nameLike) {
        var rows = await connection.SelectAsync<DateTime>(
            "SELECT CreationTime FROM QueueEntity WHERE Name LIKE ?", $"%{nameLike}%");
        return await Get(connection, rows.Select(r => r.Item1).ToList());
    }
    public async Task<AudioQueue?> GetCurrent() {
        using(var db = await _db.GetReader()) {
            return await GetCurrent(db.Connection);
        }
    }
    public async Task<AudioQueue?> Get(string name) {
        using(var db = await _db.GetReader()) {
            return await Get(db.Connection, name);
        }
    }
    public async Task<AudioQueue?> Get(DateTime creationTime) {
        using (var db = await _db.GetReader()) {
            return await Get(db.Connection, creationTime);
        }
    }
    public async Task<bool> Exists(string name) {
        using(var db = await _db.GetReader()) {
            return await Exists(db.Connection, name);
        }
    }
    public async Task<List<AudioQueue>> GetAll() {
        using(var db = await _db.GetReader()) {
            return await GetAll(db.Connection);
        }
    }
    public async Task<List<AudioQueue>> Query(string nameLike) {
        using(var db = await _db.GetReader()) {
            return await Query(db.Connection, nameLike);
        }
    }
}
