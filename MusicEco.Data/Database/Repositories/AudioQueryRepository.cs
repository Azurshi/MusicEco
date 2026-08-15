using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal class AudioQueryRepository {
    private readonly DatabaseContextAsync _db;
    public AudioQueryRepository(DatabaseContextAsync db) {
        this._db = db;
    }
    public async Task<List<AlbumData>> QueryAlbum(string nameLike) {
        using(var db = await this._db.GetReader()) {
            var now = DateTime.UtcNow;
            var rows = await db.Connection.SelectAsync<
                string, Hash256, string>($"""
                SELECT Album, FileHash, DisplayTitle
                FROM AudioEntity
                WHERE Album LIKE ?
                """, $"%{nameLike}%");
            Dictionary<string, AlbumData> map = [];
            foreach(var row in rows) {
                if (!map.TryGetValue(row.Item1, out var data)) {
                    data = new(row.Item1, now, new List<AudioEntry>());
                    map[row.Item1] = data;
                }
                var audios = (List<AudioEntry>)data.Audios;
                audios.Add(new(row.Item2, row.Item3));
            }
            return map.Values.OrderBy(d => d.Name).ToList();
        }
    }
    public async Task<AlbumData?> GetAlbum(string name) {
        using(var db = await this._db.GetReader()) {
            var now = DateTime.UtcNow;
            var rows = await db.Connection.SelectAsync<
                string, Hash256, string>($"""
                SELECT Album, FileHash, DisplayTitle
                FROM AudioEntity
                WHERE Album = ?
                """, $"{name}");
            if (rows.Count > 0) {
                List<AudioEntry> audios = [];
                foreach(var row in rows) {
                    audios.Add(new(row.Item2, row.Item3));
                }
                return new(name, now, audios);
            } else {
                return null;
            }
        }
    }
    public async Task<List<AudioEntry>> GetNotPlay(float minRatio, string nameLike) {
        using(var db = await this._db.GetReader()) {
            var rows = await db.Connection.SelectAsync<
                Hash256, string>($"""
                SELECT FileHash, DisplayTitle
                FROM AudioEntity
                WHERE Name LIKE ? AND FileHash IN (
                    SELECT UNIQUE(FileHash)
                    FROM PlayEventEntity
                    WHERE PlayedRatio >= ?
                )
                """, $"%{nameLike}%", minRatio);
            List<AudioEntry> result = [];
            foreach(var row in rows) {
                result.Add(new(row.Item1, row.Item2));
            }
            return result;
        }
    }
    public async Task<List<PlayHistoryData>> GetPlayHistory(float minRatio) {
        using(var db = await this._db.GetReader()) {
            var rows = await db.Connection.SelectAsync<
                Hash256, string, DateTime>($"""
                SELECT
                    p.FileHash,
                    (
                        SELECT a.DisplayTitle
                        FROM AudioEntity AS a
                        WHERE a.FileHash = p.FileHash
                    ),
                    MAX(p.Time) AS Time
                FROM PlayEventEntity AS p
                WHERE PlayedRatio >= ?
                GROUP BY p.FileHash
                ORDER BY Time DESC
                """, minRatio);
            List<PlayHistoryData> result = [];
            foreach(var row in rows) {
                result.Add(new(new(row.Item1, row.Item2), row.Item3));
            }
            return result;
        }
    }
    public async Task<List<PlayCountData>> GetPlayCount(float minRatio, DateTime fromTime, DateTime toTime) {
        using(var db = await this._db.GetReader()) {
            var rows = await db.Connection.SelectAsync<
                Hash256, string, int>($"""
                SELECT 
                    p.FileHash,
                    (
                        SELECT a.DisplayTitle
                        FROM AudioEntity AS a
                        WHERE a.FileHash = p.FileHash
                    ),
                    COUNT(*) AS PlayCount
                FROM PlayEventEntity AS p
                WHERE PlayedRatio >= ? AND Time >= ? AND Time <= ?
                GROUP BY p.FileHash
                ORDER BY PlayCount DESC
                """, minRatio, fromTime, toTime);
            List<PlayCountData> result = [];
            foreach(var row in rows) {
                result.Add(new(new(row.Item1, row.Item2), row.Item3));
            }
            return result;
        }
    }
}
