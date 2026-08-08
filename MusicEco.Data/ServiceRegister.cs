using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Entities;
using MusicEco.Data.Database.Relations;
using MusicEco.Data.Database.Repositories;
using MusicEco.Data.Services;
using SQLiteORM;
using SQLiteORM.Internal;
using SQLitePCL;
using System.Diagnostics;

namespace MusicEco.Data;

public static class ServiceRegister {
    public static IServiceCollection RegisterData(this IServiceCollection services) {
        services.AddSingleton<IAppSetting, AppSetting>();
        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<IFavouriteService, FavouriteService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IPlayEventService, PlayEventService>();
        services.AddSingleton<IQueueService, QueueService>();
        services.AddSingleton<IScanner, Scanner>();
        services.AddSingleton<IScanPathService, ScanPathService>();
        services.AddSingleton<IAudioQueryService, AudioQueryService>();

        services.AddSingleton<DatabaseContextAsync>();
        services.AddSingleton<AudioRepository>();
        services.AddSingleton<DictionaryRepository>();
        services.AddSingleton<FileRepository>();
        services.AddSingleton<IconRepository>();
        services.AddSingleton<PlayEventRepository>();
        services.AddSingleton<PlaylistRepository>();
        services.AddSingleton<QueueRepository>();
        services.AddSingleton<AudioQueryRepository>();

        var result = TypeMapExtend.Register(
            typeof(Hash256),
            SQLiteKeyword.Blob,
            Hash256Binder,
            Hash256Reader
            );
        result &= TypeMapExtend.Register(
            typeof(AudioTagType),
            SQLiteKeyword.Integer,
            TagTypeBinder,
            TagTypeReader
            );
        result &= TypeMapExtend.Register(
            typeof(Memory<byte>),
            SQLiteKeyword.Blob,
            MemoryBinder,
            MemoryReader
            );
        if (result == false) {
            Debug.WriteLine("Failed to register database field");
        }
        return services;
    }
    private static int Hash256Binder(sqlite3_stmt stmt, int index, object value) {
        Hash256 hash = (Hash256)value;
        return raw.sqlite3_bind_blob(stmt, index, hash.AsReadOnlySpan());
    }
    private static object Hash256Reader(sqlite3_stmt stmt, int index) {
        ReadOnlySpan<byte> span = raw.sqlite3_column_blob(stmt, index);
        Hash256 hash = new();
        span.CopyTo(hash.AsSpan());
        return hash;
    }
    private static readonly Memory<byte> MemoryBuffer = new byte[Config.LargeIconBufferSize];
    private static int MemoryBinder(sqlite3_stmt stmt, int index, object value) {
        Memory<byte> memory = (Memory<byte>)value;
        return raw.sqlite3_bind_blob(stmt, index, memory.Span);
    }
    private static object MemoryReader(sqlite3_stmt stmt, int index) {
        ReadOnlySpan<byte> span = raw.sqlite3_column_blob(stmt, index);
        span.CopyTo(MemoryBuffer.Span);
        return MemoryBuffer[..span.Length];
    }
    private static int TagTypeBinder(sqlite3_stmt stmt, int index, object value) {
        AudioTagType tagType = (AudioTagType)value;
        return raw.sqlite3_bind_int(stmt, index, (int)tagType);
    }
    private static object TagTypeReader(sqlite3_stmt stmt, int index) {
        int value = raw.sqlite3_column_int(stmt, index);
        return (AudioTagType)value;
    }
    private static string SaveDirectory = string.Empty;
    public static async Task Initialize(IServiceProvider provider, string saveDirectory) {
        SQLitePCL.Batteries.Init();
        SaveDirectory = saveDirectory;
        DatabaseContextAsync db = provider.GetRequiredService<DatabaseContextAsync>();
        string dbPath = Path.Join(SaveDirectory, Config.DatabaseName);
        List<Type> tables = [
            typeof(AudioEntity), typeof(DictionaryEntry),
            typeof(FileEntity), typeof(IconEntity),
            typeof(PlayEventEntity), typeof(PlaylistEntity),
            typeof(QueueEntity),

            typeof(AudioTagRelation),
            typeof(PlaylistAudioRelation),
            typeof(QueueAudioRelation)
            ];
        await db.StartDatabase(dbPath, Config.NumDatabaseReader, tables, [], true);
    }
    internal static SQLiteReadConnection GetReader() {
        string dbPath = Path.Join(SaveDirectory, Config.DatabaseName);
        return new SQLiteReadConnection(dbPath);
    }
}
