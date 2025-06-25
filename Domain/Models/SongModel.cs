namespace Domain.Models;
public interface ISongModel: IBaseModel {
    public string Title { get; set; }
    public string Album { get; set; }
    public string AlbumArtist { get; set; }
    public string Comment { get; set; }
    public string Composer { get; set; }
    public string Performer { get; set; }
    public string Genre { get; set; }
    public string Lyric { get; set; }
    public int Disc { get; set; }
    public int DiscCount { get; set; }
    public int Track { get; set; }
    public int TrackCount { get; set; }
    public int Year { get; set; }

    public int PlayCount { get; set; }
    public bool Favourite { get; set; }
    public DateTime? LastPlayed { get; set; }
    /// <summary>
    /// Utility property from <see cref="File"/>
    /// </summary>
    public bool Available { get; }

    /// <summary>
    /// Get available file, prefer local over network
    /// </summary>
    public IFileModel? File { get; }
    public abstract void AddFile(IFileModel file);
    public abstract void RemoveFile(IFileModel file);
    public abstract List<IFileModel> GetFiles();
}