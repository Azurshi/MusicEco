namespace Domain.Models; 
public class DefaultValue {
    public const string Unknow = "Unknow";
    public const string Empty = "";
    public const long Id = -1;
    public const int Count = 0;
    public const SettingFieldFormat FieldFormat = SettingFieldFormat.Null;

    public const string Playlist = "Playlist";
    public const string Queue = "Queue";
    public static readonly IReadOnlyList<string> PlaylistType = [Playlist, Queue];

    public const string Audio = "Audio";
    public const string Image = "Image";
    public const string Other = "Other";
    public static readonly IReadOnlyList<string> FileType = [Audio, Image, Other];
}
