namespace Domain.Models;
public interface IPlaylistModel: IBaseModel {
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime? LastPlayed { get; set; }
    public int Order { get; set; }

    public IReadOnlyList<ISongModel> Songs { get;}
    public IReadOnlyList<ISongModel> AvailableSongs { get; }
    /// <summary>
    /// Return current <see cref="ISongModel"/> if available
    /// </summary>
    public ISongModel? Current { get; set; }
    /// <summary>
    /// Return next availble <see cref="ISongModel"/>, default to <see cref="FirstSong"/>
    /// </summary>
    public ISongModel? NextSong { get; }
    /// <summary>
    /// Return previous availble <see cref="ISongModel"/>, default to <see cref="FirstSong"/>
    /// </summary>
    public ISongModel? PreviousSong { get; }
    /// <summary>
    /// Return frist availble <see cref="ISongModel"/>, default to null
    /// </summary>
    public ISongModel? FirstSong { get; }

    public void AddSong(ISongModel song);
    public void RemoveSong(ISongModel song);
    public void InsertSong(int index, ISongModel song);
    public void ConsolideOrder(string? type);
    public bool IsEndOfList();
    public void Shuffle();
}