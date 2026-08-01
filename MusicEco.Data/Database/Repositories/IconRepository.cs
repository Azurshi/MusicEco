using Blake3;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using SQLiteORM;
using System.Numerics;

namespace MusicEco.Data.Database.Repositories;

internal class IconRepository {
    private readonly DatabaseContextAsync _db;
    private readonly SemaphoreSingle _dataLimiter;
    private readonly IIconEncoder _encoder;
    public IconRepository(DatabaseContextAsync dbContext, IIconEncoder iconEncoder) {
        this._db = dbContext;
        this._dataLimiter = new();
        this._encoder = iconEncoder;
        this._encoder.Initialize(1);
    }
    public static async Task<Hash256?> GetCoverHash(SQLiteReadConnection connection, Hash256 fileHash) {
        var rows = await connection.SelectAsync<Hash256?>(
            "SELECT IconHash FROM AudioEntity WHERE FileHash = ?", fileHash);
        if (rows.Count > 0) {
            return rows[0].Item1;
        }
        else {
            return null;
        }
    }
    public async Task<int> GetCoverData(SQLiteReadConnection connection, Hash256 iconHash, CoverSize size, byte[] buffer) {
        string columnName = size switch {
            CoverSize.Small => "SmallIcon",
            CoverSize.Medium => "MediumIcon",
            CoverSize.Large => "LargeIcon",
            _ => throw new ArgumentOutOfRangeException(nameof(size))
        };
        await this._dataLimiter.WaitAsync();
        try {
            var rows = await connection.SelectAsync<Memory<byte>>(
                $"SELECT {columnName} FROM IconEntity WHERE Hash = ?", iconHash);
            if (rows.Count > 0) {
                var row = rows[0];
                if (buffer.Length < row.Item1.Length) {
                    throw new Exception("Buffer overflow");
                }
                row.Item1.CopyTo(buffer);
                return row.Item1.Length;
            }
            else {
                return 0;
            }
        }
        finally {
            this._dataLimiter.Release();
        }
    }
    public async Task<Hash256?> GetCoverHash(Hash256 fileHash) {
        using(var db = await _db.GetReader()) {
            return await GetCoverHash(db.Connection, fileHash);
        }
    }
    public async Task<int> GetCoverData(Hash256 iconHash, CoverSize size, byte[] buffer) {
        using(var db = await _db.GetReader()) {
            return await GetCoverData(db.Connection, iconHash, size, buffer);
        }
    }
}
