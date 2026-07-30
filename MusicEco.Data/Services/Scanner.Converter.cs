using MusicEco.Core.Data;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Entities;
using MusicEco.Data.Database.Relations;

namespace MusicEco.Data.Services;

internal partial class Scanner {
    private static ValueTuple<AudioEntity, List<AudioTagRelation>> AudioMetadataToDatabaseEntry(Hash256 fileHash, Hash256? iconHash, AudioMetadata metadata) {
        AudioEntity entity = new((
            fileHash, iconHash, metadata.Duration,
            metadata.Title ?? string.Empty, metadata.Title, metadata.TitleSort,
            metadata.Album, metadata.AlbumSort, metadata.Comment,
            metadata.Year, metadata.Year, metadata.TrackCount, metadata.Disc,
            metadata.Disc, metadata.Lyrics, metadata.Grouping, metadata.BeatsPerMinute,
            metadata.Copyright, metadata.DateTagged, metadata.InitialKey, metadata.ISRC));
        List<AudioTagRelation> tagRelations = [];
        for (int i = 0; i < metadata.Artists.Count; i++) {
            tagRelations.Add(new((AudioTagType.Artist, fileHash, metadata.Artists[i], i)));
        }
        for (int i = 0; i < metadata.ArtistsSort.Count; i++) {
            tagRelations.Add(new((AudioTagType.ArtistSort, fileHash, metadata.ArtistsSort[i], i)));
        }
        for (int i = 0; i < metadata.AlbumArtists.Count; i++) {
            tagRelations.Add(new((AudioTagType.AlbumArtist, fileHash, metadata.AlbumArtists[i], i)));
        }
        for (int i = 0; i < metadata.AlbumArtistsSort.Count; i++) {
            tagRelations.Add(new((AudioTagType.AlbumArtistSort, fileHash, metadata.AlbumArtistsSort[i], i)));
        }
        for (int i = 0; i < metadata.Composers.Count; i++) {
            tagRelations.Add(new((AudioTagType.Composer, fileHash, metadata.Composers[i], i)));
        }
        for (int i = 0; i < metadata.ComposersSort.Count; i++) {
            tagRelations.Add(new((AudioTagType.ComposerSort, fileHash, metadata.ComposersSort[i], i)));
        }
        for (int i = 0; i < metadata.Genres.Count; i++) {
            tagRelations.Add(new((AudioTagType.Genre, fileHash, metadata.Genres[i], i)));
        }
        return (entity, tagRelations);
    }
    private static FileEntity FileEntryToEntity(FileEntry file) {
        return new((file.Path, file.Hash, file.ModifiedTime, file.Name, file.Extension, file.Size));
    }

#if WINDOWS
    private static FileEntry FileInfoToEntry(FileInfo file, byte[] ioBuffer) {
        Hash256 hash;
        using (var fileStream = file.OpenRead()) {
            hash = ComputeHash(fileStream, ioBuffer);
        }
        string fileName = Path.GetFileNameWithoutExtension(file.Name);
        FileEntry entry = new(file.FullName, hash, file.LastWriteTimeUtc, file.Name, fileName, file.Length);
        return entry;
    }
#endif
}
