using MusicEco.Common.Events;
using MusicEco.Global;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MusicEco.Common;
public static class Setting {
    public static bool AutoSave = true;
    public static string DataFolderPath = Global.AbstractLayers.File.GetPersPath("data");
    public static string[] SupportedExtension = [
            ".mp3",
            ".wav",
            ".flac"
        ];

    public const int ListFrameAmount = 20;
    public const int GridHeightMulti = 3;
    public const int GridCoumnsCount = 4;
    public const int MinimumSearchLenth = 3;
    public const int ButtonSize = 50;
    public static readonly Vector2I MiniMenuSize = new(150, 25);
    public static readonly Vector2I ProgressIconSize = new(30, 30);
    public static readonly Vector2I ControlButtonSize = new(50, 50);
    public static readonly Vector2I ImageIconResolution = new(128, 128);

    public static readonly bool ScannerOverwrite = false;
    public static readonly JsonSerializerOptions SerializeOption = new() {
        IncludeFields = false,
        IgnoreReadOnlyProperties = true,
        IgnoreReadOnlyFields = true
    };
}

