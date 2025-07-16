using Domain;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataStorage; 
public static class Config {
    public static readonly List<string> ImageFileExtensions = [
        ".png",
        ".jpg",
        ".jpeg"
    ];

    internal static string DataFolderPath => CustomFile.GetPersPath("data");
    /// <summary>
    /// ms
    /// </summary>
    public static int AutoSaveDelay = 1000;
    public static bool PrintGlobalString = false;
    public static bool LogGlobalState = true;
    public static bool LogClassSate = true;
    internal static readonly JsonSerializerOptions SerializeOption = new() {
        IncludeFields = false,
        IgnoreReadOnlyProperties = true,
        IgnoreReadOnlyFields = true,
        Converters = {
            new JsonStringEnumConverter()
        }
    };
    internal static string GetSaveFilePath(string className) {
        return CustomFile.Join(Config.DataFolderPath, className + ".json");
    }
#if WINDOWS
    public static Vector2I IconSize = new(256, 256);
#else
    public static Vector2I IconSize = new(128, 128);
#endif

}
