using DataStorage.Models;
using Domain.Models;
using System.Diagnostics;
#if ANDROID
using MusicEco.Platforms.Android;
#endif
namespace DataStorage.DataAccess; 
internal static class ScannerFileHelper {
    internal static SongModel ReadMetaData(string filePath) {
        try {
#if WINDOWS
            TagLib.File file = TagLib.File.Create(filePath, TagLib.ReadStyle.PictureLazy);
#elif ANDROID
            Android.Net.Uri fileUri = Android.Net.Uri.Parse(filePath)!;
            TagLib.File file = TagLibCreatFile(fileUri);
#endif
            SongModel song = new() {
                Title = file.Tag.Title ?? DefaultValue.Empty,
                Album = file.Tag.Album ?? DefaultValue.Empty,
                AlbumArtist = string.Join(", ", file.Tag.AlbumArtists),
                Composer = string.Join(", ", file.Tag.Composers),
                Performer = string.Join(", ", file.Tag.Performers),
                Comment = file.Tag.Comment ?? DefaultValue.Empty,
                Disc = (int)file.Tag.Disc,
                DiscCount = (int)file.Tag.DiscCount,
                Genre = string.Join(", ", file.Tag.Genres),
                Track = (int)file.Tag.Track,
                TrackCount = (int)file.Tag.TrackCount,
                Year = (int)file.Tag.Year,
                Lyric = file.Tag.Lyrics ?? DefaultValue.Empty
            };
            return song;
        }
        catch {
            Debug.WriteLine($"Failed to extract tag of {filePath}");
            return new SongModel();
        }
    }
#if ANDROID
    internal static TagLib.File TagLibCreatFile(Android.Net.Uri uri) {
        try {
            using (var stream = GetStreamFromUri(uri)) {
                if (stream == null) {
                    throw new Exception("Failed to read file");
                }
                var file = TagLib.File.Create(new PlatfformStreamFileAbstraction(uri, stream));
                return file;
            }
        }
        catch {
            throw new NotImplementedException();
        }
    }
    internal static Stream? GetStreamFromUri(Android.Net.Uri uri) {
        var contextResolver = Android.App.Application.Context.ContentResolver;
        return contextResolver!.OpenInputStream(uri);
    }
#endif
}
#if ANDROID
public class PlatfformStreamFileAbstraction(Android.Net.Uri uri, Stream stream) : TagLib.File.IFileAbstraction {
    private readonly Android.Net.Uri _uri = uri;
    private readonly Stream _stream = stream;

    public string Name => _uri.ToString()!;
    public Stream ReadStream => _stream;
    public Stream WriteStream => throw new NotImplementedException();
    public void Dispose() {
        _stream?.Dispose();
    }
    public void CloseStream(Stream stream) {
        Dispose();
    }
}
#endif
