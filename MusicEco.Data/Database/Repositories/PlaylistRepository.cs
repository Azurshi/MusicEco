using MusicEco.Core.Data;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Entities;
using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal class PlaylistRepository {
    private readonly DatabaseContextAsync _db;
    public PlaylistRepository(DatabaseContextAsync dbContext) {
        this._db = dbContext;
    }
    public static async Task<List<AudioPlaylist>> Get(SQLiteReadConnection connection, List<DateTime> creationTimes) {
        List<AudioPlaylist> playlists = [];
        if (creationTimes.Count == 0) {
            return playlists;
        }
        var creationTimeObjs = creationTimes.Cast<object>().ToArray();
        var rows = await connection.SelectAsync<
            DateTime, string, DateTime, DateTime>($"""
            SELECT * FROM PlaylistEntity 
            WHERE CreationTime IN {Config.GetPlaceholder(creationTimeObjs.Length)}
            """, creationTimeObjs);
        List<ValueTuple<DateTime, Hash256>> hashRows = [];
        foreach (var batchObjs in creationTimeObjs.Chunk(Config.MaxParameterCount)) {
            var placeholder = Config.GetPlaceholder(batchObjs.Length);
            var batchResult = await connection.SelectAsync<DateTime, Hash256>($"""
                SELECT CreationTime, FileHash
                FROM PlaylistAudioRelation
                ORDER BY OrderIndex
                WHERE CreationTime IN ({placeholder})
                """, batchObjs);
            hashRows.AddRange(batchResult);
        }
        var hashes = hashRows.Select(r => r.Item2).Distinct().ToList();
        List<AudioEntry> audios = await AudioRepository.GetEntry(connection, hashes);
        Dictionary<Hash256, AudioEntry> audioMap = audios.ToDictionary(a => a.Hash);
        Dictionary<DateTime, List<AudioEntry>> entriesMap = [];
        foreach (var (creationTime, hash) in hashRows) {
            if (!entriesMap.TryGetValue(creationTime, out var entries)) {
                entries = [];
                entriesMap[creationTime] = entries;
            }
            entries.Add(audioMap[hash]);
        }
        foreach (var row in rows) {
            PlaylistEntity entity = new(row);
            AudioPlaylist playlist = new(entity.Name, entity.CreationTime, entity.ModifiedTime, entity.LastPlayTime, entriesMap[entity.CreationTime]);
            playlists.Add(playlist);
        }
        return playlists;
    }
    public static async Task<AudioPlaylist?> Get(SQLiteReadConnection connection, DateTime creationTime) {
        var playlists = await Get(connection, [creationTime]);
        if (playlists.Count > 0) {
            return playlists[0];
        } else {
            return null;
        }
    }
    public static async Task<List<AudioPlaylist>> GetAll(SQLiteReadConnection connection) {
        var rows = await connection.SelectAsync<DateTime>(
            "SELECT CreationTime FROM PlaylistEntity");
        var creationTimes = rows.Select(r => r.Item1).ToList();
        return await Get(connection, creationTimes);
    }
    public static async Task<List<AudioPlaylist>> Query(SQLiteReadConnection connection, string nameLike) {
        List<AudioPlaylist> playlists = [];
        var rows = await connection.SelectAsync<DateTime>(
            "SELECT CreationTime FROM PlaylistEntity WHERE Name LIKE ?", $"%{nameLike}%");
        var creationTimes = rows.Select(r => r.Item1).ToList();
        return await Get(connection, creationTimes);
    }
    public async Task<AudioPlaylist?> Get(DateTime creationTime) {
        using(var db = await _db.GetReader()) {
            return await Get(db.Connection, creationTime);
        }
    }
    public async Task<List<AudioPlaylist>> GetAll() {
        using(var db = await _db.GetReader()) {
            return await GetAll(db.Connection);
        }
    }
    public async Task<List<AudioPlaylist>> Query(string nameLike) {
        using(var db = await _db.GetReader()) {
            return await Query(db.Connection, nameLike);
        }
    }
}
