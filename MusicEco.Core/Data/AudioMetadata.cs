
namespace MusicEco.Core.Data;

public class AudioMetadata {
    public string? Title { get; init; } // Ok
    public string? TitleSort { get; init; } // Ok
    public IReadOnlyList<string> Artists { get; init; } // Artist
    public IReadOnlyList<string> ArtistsSort { get; init; } // Pl
    public IReadOnlyList<string> AlbumArtists { get; init; } // Ok
    public IReadOnlyList<string> AlbumArtistsSort { get; init; } // Ok
    public IReadOnlyList<string> Composers { get; init; } // Ok
    public IReadOnlyList<string> ComposersSort { get; init; } // Ok
    public string? Album { get; init; } // Ok
    public string? AlbumSort { get; init; } // Ok
    public string? Comment { get; init; } // Ok
    public IReadOnlyList<string> Genres { get; init; } // Ok
    public int? Year { get; init; } // Ok
    public int? Track { get; init; } // Ok
    public int? TrackCount { get; init; } // Ok
    public int? Disc { get; init; } // Ok
    public int? DiscCount { get; init; } // Ok
    public string? Lyrics { get; init; } // Ok
    public string? Grouping { get; init; }
    public int? BeatsPerMinute { get; init; }
    public string? Copyright { get; init; }
    public DateTime? DateTagged { get; init; }
    public string? InitialKey { get; init; }
    public string? ISRC { get; init; }

    public TimeSpan Duration { get; init; }

    public AudioMetadata(string? title, string? titleSort, IReadOnlyList<string> artists, IReadOnlyList<string> artistsSort, IReadOnlyList<string> albumArtists, IReadOnlyList<string> albumArtistsSort, IReadOnlyList<string> composers, IReadOnlyList<string> composersSort, string? album, string? albumSort, string? comment, IReadOnlyList<string> genres, int? year, int? track, int? trackCount, int? disc, int? discCount, string? lyrics, string? grouping, int? beatsPerMinute, string? copyright, DateTime? dateTagged, string? initialKey, string? iSRC, TimeSpan duration) {
        this.Title = title;
        this.TitleSort = titleSort;
        this.Artists = artists;
        this.ArtistsSort = artistsSort;
        this.AlbumArtists = albumArtists;
        this.AlbumArtistsSort = albumArtistsSort;
        this.Composers = composers;
        this.ComposersSort = composersSort;
        this.Album = album;
        this.AlbumSort = albumSort;
        this.Comment = comment;
        this.Genres = genres;
        this.Year = year;
        this.Track = track;
        this.TrackCount = trackCount;
        this.Disc = disc;
        this.DiscCount = discCount;
        this.Lyrics = lyrics;
        this.Grouping = grouping;
        this.BeatsPerMinute = beatsPerMinute;
        this.Copyright = copyright;
        this.DateTagged = dateTagged;
        this.InitialKey = initialKey;
        this.ISRC = iSRC;
        this.Duration = duration;
    }
}