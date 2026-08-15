using System.Numerics;

namespace MusicEco.Data;

public static class Config {
    public const int MaxIconSize = 4096;
    public const int MaxParameterCount = 32766;
    public const int SmallIconDimension = 64 * 64;
    public const int MediumIconDimension = 256 * 256;
    public const int LargeIconDimension = 1024 * 768;
    public static readonly Vector2 LargeIconSize = new(1024, LargeIconDimension / 1024);
    public static readonly Vector2 MediumIconSize = new(256, MediumIconDimension / 256);
    public static readonly Vector2 SmallIconSize = new(64, SmallIconDimension / 64);
    public const int SmallIconBufferSize = SmallIconDimension * 4;
    public const int MediumIconBufferSize = MediumIconDimension * 4;
    public const int LargeIconBufferSize = LargeIconDimension * 4;

    public const int SaveDelayMs = 1000 * 1;
    public const int SaveLoopMs = 100;

    public const int IOBufferSize = 1 * 1024 * 1024;
#if ANDROID
    public const int TagLibIOBufferSize = 64 * 1024;
#endif
    public const int ScannerIconBufferInitialSize = 1 * 1024 * 1024;

    public const string DatabaseName = "appData.db";
    public const int NumDatabaseReader = 2;

    internal static string GetPlaceholder(int count) {
        return string.Join(",", Enumerable.Repeat("?", count));
    }
}
