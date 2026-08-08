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
}
