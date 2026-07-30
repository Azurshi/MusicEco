using MusicEco.Core.Data;
#if ANDROID
using MusicEco.Core.Platforms.Android;
#endif
namespace MusicEco.Data.Services;

internal partial class Scanner {
    private partial class StreamFileAbstraction(string name, Stream stream): TagLib.File.IFileAbstraction, IDisposable {
        private readonly Stream _stream = stream;
        public string Name => name;

        public Stream ReadStream => _stream;

        public Stream WriteStream => throw new NotImplementedException();

        public void CloseStream(Stream stream) {
            Dispose();
        }

        public void Dispose() {
            _stream.Close();
            _stream.Dispose();
        }
    }
    private static ValueTuple<AudioMetadata, TagLib.ByteVector?> ReadMetadata(string filePath) {
        Stream stream;
#if WINDOWS
        stream = File.OpenRead(filePath);
#elif ANDROID
        Android.Net.Uri uri = Android.Net.Uri.Parse(filePath)!;
        stream = UriUtility.OpenFile(uri, Config.TagLibIOBufferSize, FileAccess.Read);
#else
        stream = File.OpenRead(filePath); // Not implemented
#endif
        // Dynamic allocation here
        using (var file = TagLib.File.Create(new StreamFileAbstraction(filePath, stream), TagLib.ReadStyle.Average)) {
            var tag = file.Tag;
            AudioMetadata metadata = new(
                tag.Title, tag.TitleSort,
                tag.Performers, tag.PerformersSort,
                tag.AlbumArtists, tag.AlbumArtistsSort,
                tag.Composers, tag.ComposersSort,
                tag.Album, tag.AlbumSort,
                tag.Comment, tag.Genres, (int?)tag.Year,
                (int?)tag.Track, (int?)tag.TrackCount,
                (int?)tag.Disc, (int?)tag.DiscCount,
                tag.Lyrics, tag.Grouping, (int?)tag.BeatsPerMinute,
                tag.Copyright, tag.DateTagged,
                tag.InitialKey, tag.ISRC,
                file.Properties.Duration);
            if (tag.Pictures.Length > 0) {
                var data = tag.Pictures[0].Data;
                return (metadata, data);
            }
            else {
                return (metadata, null);
            }
;        }
    }
}