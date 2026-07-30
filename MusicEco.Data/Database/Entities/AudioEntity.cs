using MusicEco.Core.Types;
using SQLiteORM;

namespace MusicEco.Data.Database.Entities;

[Table(
    TableName = null,
    IndexOptions = ["DisplayTitle", "Title", "IconHash"],
    TableOptions = [],
    AfterTableOption = ""
)]
internal class AudioEntity {
    [PrimaryKey] public Hash256 FileHash { get; init; }
    [DatabaseField] public Hash256? IconHash { get; init; }
    [DatabaseField] public TimeSpan Duration { get; init; }
    [DatabaseField] public string DisplayTitle { get; init; }
    [DatabaseField] public string? Title { get; init; }
    [DatabaseField] public string? TitleSort { get; init; }
    [DatabaseField] public string? Album { get; init; }
    [DatabaseField] public string? AlbumSort { get; init; }
    [DatabaseField] public string? Comment { get; init; }
    [DatabaseField] public int? Year { get; init; }
    [DatabaseField] public int? Track { get; init; }
    [DatabaseField] public int? TrackCount { get; init; }
    [DatabaseField] public int? Disc { get; init; }
    [DatabaseField] public int? DiscCount { get; init; }
    [DatabaseField] public string? Lyrics { get; init; }
    [DatabaseField] public string? Grouping { get; init; }
    [DatabaseField] public int? BeatsPerMinute { get; init; }
    [DatabaseField] public string? Copyright { get; init; }
    [DatabaseField] public DateTime? DateTagged { get; init; }
    [DatabaseField] public string? InitialKey { get; init; }
    [DatabaseField] public string? ISRC { get; init; }

    public AudioEntity((
        Hash256 FileHash, Hash256? IconHash, TimeSpan Duration, string DisplayTitle,
        string? Title, string? TitleSort,
        string? Album, string? AlbumSort,
        string? Comment, int? Year, 
        int? Track, int? TrackCount,
        int? Disc, int? DiscCount,
        string? Lyrics, string? Grouping,
        int? BeatsPerMinute, string? Copyright, DateTime? DateTagged,
        string? InitialKey, string? ISRC
        ) t) {
        this.FileHash = t.FileHash;
        this.IconHash = t.IconHash;
        this.Duration = t.Duration;
        this.DisplayTitle = t.DisplayTitle;
        this.Title = t.Title;
        this.TitleSort = t.TitleSort;
        this.Album = t.Album;
        this.AlbumSort = t.AlbumSort;
        this.Comment = t.Comment;
        this.Year = t.Year;
        this.Track = t.Track;
        this.TrackCount = t.TrackCount;
        this.Disc = t.Disc;
        this.DiscCount = t.DiscCount;
        this.Lyrics = t.Lyrics;
        this.Grouping = t.Grouping;
        this.BeatsPerMinute = t.BeatsPerMinute;
        this.Copyright = t.Copyright;
        this.DateTagged = t.DateTagged;
        this.InitialKey = t.InitialKey;
        this.ISRC = t.ISRC;
    }
}
