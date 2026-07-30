using MusicEco.Core.Data;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Entities;
using SQLiteORM;

namespace MusicEco.Data.Database.Repositories;

internal class FileRepository {
    private readonly DatabaseContextAsync _db;
    public FileRepository(DatabaseContextAsync dbContext) {
        this._db = dbContext;
    }
    private static FileEntry ToEntry(FileEntity entity) {
        return new(entity.Path, entity.Hash, entity.ModifiedTime, entity.Name, entity.Extension, entity.Size);
    }
    public static async Task<FileEntry?> Get(SQLiteReadConnection connection, string path) {
        var rows = await connection.SelectAsync<
            string, Hash256, DateTime, string, string, long>(
            "SELECT * FROM FileEntity WHERE Path = ?", path);
        if (rows.Count > 0) {
            FileEntity entity = new(rows[0]);
            return ToEntry(entity);
        }
        else {
            return null;
        }
    }
    public static async Task<List<FileEntry>> Query(SQLiteReadConnection connection, string path) {
        List<FileEntry> files = [];
        var rows = await connection.SelectAsync<
            string, Hash256, DateTime, string, string, long>(
            "SELECT * FROM FileEntity WHERE Path LIKE ?", $"{path}%");
        foreach(var row in rows) {
            FileEntity entity = new(row);
            files.Add(ToEntry(entity));
        }
        return files;
    }
    public static async Task<List<FileEntry>> GetAll(SQLiteReadConnection connection) {
        List<FileEntry> files = [];
        var rows = await connection.SelectAsync<
            string, Hash256, DateTime, string, string, long>(
            "SELECT * FROM FileEntity");
        foreach (var row in rows) {
            FileEntity entity = new(row);
            files.Add(ToEntry(entity));
        }
        return files;
    }
    public static async Task<List<FileEntry>> GetByHash(SQLiteReadConnection connection, Hash256 hash) {
        List<FileEntry> files = [];
        var rows = await connection.SelectAsync<
            string, Hash256, DateTime, string, string, long>(
            "SELECT * FROM FileEntity WHERE Hash = ?", hash);
        foreach(var row in rows) {
            FileEntity entity = new(row);
            files.Add(ToEntry(entity));
        }
        return files;
    }
    public async Task<FileEntry?> Get(string path) {
        using(var db = await _db.GetReader()) {
            return await Get(db.Connection, path);
        }
    }
    public async Task<List<FileEntry>> Query(string path) {
        using (var db = await _db.GetReader()) {
            return await Query(db.Connection, path);
        }
    }
    public async Task<List<FileEntry>> GetAll() {
        using(var db = await _db.GetReader()) {
            return await GetAll(db.Connection);
        }
    }
    public async Task<List<FileEntry>> GetByHash(Hash256 fileHash) {
        using(var db = await _db.GetReader()) {
            return await GetByHash(fileHash);
        }
    }
}
