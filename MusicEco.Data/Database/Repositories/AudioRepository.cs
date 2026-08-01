using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Entities;
using MusicEco.Data.Database.Relations;
using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal class AudioRepository {
    private readonly DatabaseContextAsync _db;
    public AudioRepository(DatabaseContextAsync db) {
        this._db = db;
    }
    private sealed record MetadataRelationDto(
        List<string> Artists, List<string> ArtistsSort,
        List<string> AlbumArtists, List<string> AlbumArtistsSort,
        List<string> Composers, List<string> ComposersSort,
        List<string> Genres) {
        public MetadataRelationDto() : this([], [], [], [], [], [], []) { }
        public void Add(AudioTagType tagType, string value) {
            switch (tagType) {
                case AudioTagType.Artist:
                    Artists.Add(value);
                    break;
                case AudioTagType.ArtistSort:
                    ArtistsSort.Add(value);
                    break;
                case AudioTagType.AlbumArtist:
                    AlbumArtists.Add(value);
                    break;
                case AudioTagType.AlbumArtistSort:
                    AlbumArtistsSort.Add(value);
                    break;
                case AudioTagType.Composer:
                    Composers.Add(value);
                    break;
                case AudioTagType.ComposerSort:
                    ComposersSort.Add(value);
                    break;
                case AudioTagType.Genre: 
                    Genres.Add(value);
                    break;
                default:
                    break;
            }
        }
    }
    public static async Task<List<AudioModel>> Get(SQLiteReadConnection connection, IReadOnlyList<Hash256> hashes) {
        List<AudioModel> result = [];
        foreach(var batch in hashes.Chunk(Config.MaxParameterCount)) {
            var batchObjs = batch.Cast<object>().ToArray();
            var placeholder = Config.GetPlaceholder(batch.Length);
            var mainRows = await connection.SelectAsync<
                Hash256, Hash256?, TimeSpan, string,
                string?, string?, string?, string?, string?,
                int?, int?, int?, int?, int?,
                string?, string?, int?, string?, DateTime?, string?, string?
                >($"""
                SELECT * FROM AudioEntity 
                WHERE FileHash IN ({placeholder})
                """, batchObjs);
            var relationRows = await connection.SelectAsync<
                AudioTagType, Hash256, string
                >($"""
                SELECT TagType, FileHash, Name FROM AudioTagRelation
                WHERE FileHash IN ({placeholder})
                ORDER BY OrderIndex
                """, batchObjs);
            var fileRows = await connection.SelectAsync<
                string, Hash256, DateTime, string, string, long
                >($"""
                SELECT * FROM FileEntity
                WHERE Hash IN ({placeholder})
                """, batchObjs);
            Dictionary<Hash256, MetadataRelationDto> map = [];
            foreach(var row in relationRows) {
                if (!map.TryGetValue(row.Item2, out var dto)) {
                    dto = new();
                    map[row.Item2] = dto;
                }
                dto.Add(row.Item1, row.Item3);
            }
            Dictionary<Hash256, List<FileEntry>> fileMap = [];
            foreach(var row in fileRows) {
                if (!fileMap.TryGetValue(row.Item2, out var files)) {
                    files = [];
                    fileMap[row.Item2] = files;
                }
                FileEntity entity = new(row);
                FileEntry file = new(entity.Path, entity.Hash, entity.ModifiedTime, entity.Name, entity.Extension, entity.Size);
                files.Add(file);
            }
            foreach(var row in mainRows) {
                AudioEntity entity = new(row);
                MetadataRelationDto dto = map.GetValueOrDefault(entity.FileHash, new());
                List<FileEntry> files = fileMap.GetValueOrDefault(entity.FileHash, []);
                AudioMetadata metadata = new(
                    entity.Title, entity.TitleSort,
                    dto.Artists, dto.ArtistsSort,
                    dto.AlbumArtists, dto.AlbumArtistsSort,
                    dto.Composers, dto.ComposersSort,
                    entity.Album, entity.AlbumSort,
                    entity.Comment,
                    dto.Genres,
                    entity.Year, entity.Track, entity.TrackCount, entity.Disc, entity.DiscCount,
                    entity.Lyrics, entity.Grouping, entity.BeatsPerMinute, entity.Copyright,
                    entity.DateTagged, entity.InitialKey, entity.ISRC, entity.Duration
                    );
                AudioModel model = new(entity.FileHash, metadata, files);
                result.Add(model);
            }
        }
        return result;
    }
    public static async Task<List<AudioEntry>> GetEntry(SQLiteReadConnection connection, IReadOnlyList<Hash256> hashes) {
        List<AudioEntry> result = [];
        foreach (var batch in hashes.Chunk(Config.MaxParameterCount)) {
            var batchObjs = batch.Cast<object>().ToArray();
            var placeholder = Config.GetPlaceholder(batch.Length);
            var rows = await connection.SelectAsync<Hash256, string>($"""
                SELECT FileHash, DisplayTitle FROM AudioEntity
                WHERE FileHash IN ({placeholder})
                """, batchObjs);
            foreach(var row in rows) {
                result.Add(new(row.Item1, row.Item2));
            }
        }
        return result;
    }
    public async Task<List<AudioModel>> Get(IReadOnlyList<Hash256> hashes) {
        using(var db = await _db.GetReader()) {
            return await Get(db.Connection, hashes);
        }
    }
    public async Task<List<AudioEntry>> GetEntry(IReadOnlyList<Hash256> hashes) {
        using (var db = await _db.GetReader()) {
            return await GetEntry(db.Connection, hashes);
        }
    }
    public async Task<List<AudioModel>> GetAll() {
        using (var db = await _db.GetReader()) {
            var rows = await db.Connection.SelectAsync<Hash256>(
                "SELECT FileHash FROM AudioEntity");
            List<Hash256> hashes = rows.Select(r => r.Item1).ToList();
            return await Get(db.Connection, hashes);
        }
    }
    public async Task<List<AudioEntry>> GetAllEntry() {
        using (var db = await _db.GetReader()) {
            var rows = await db.Connection.SelectAsync<Hash256>(
                "SELECT FileHash FROM AudioEntity");
            List<Hash256> hashes = rows.Select(r => r.Item1).ToList();
            return await GetEntry(db.Connection, hashes);
        }
    }
    public async Task<List<AudioModel>> Query(string nameLike) {
        using (var db = await _db.GetReader()) {
            var rows = await db.Connection.SelectAsync<Hash256>(
                "SELECT FileHash FROM AudioEntity WHERE DisplayTitle LIKE ? OR Title LIKE ?", $"%{nameLike}%", $"%{nameLike}%");
            List<Hash256> hashes = rows.Select(r => r.Item1).ToList();
            return await Get(db.Connection, hashes);
        }
    }
    public async Task<List<AudioEntry>> QueryEntry(string nameLike) {
        using (var db = await _db.GetReader()) {
            var rows = await db.Connection.SelectAsync<Hash256>(
                "SELECT FileHash FROM AudioEntity WHERE DisplayTitle LIKE ? OR Title LIKE ?", $"%{nameLike}%", $"%{nameLike}%");
            List<Hash256> hashes = rows.Select(r => r.Item1).ToList();
            return await GetEntry(db.Connection, hashes);
        }
    }
}
