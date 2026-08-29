using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Overlays;

public partial class AudioInfoOverlayViewModel: BaseOverlayViewModel {
    private readonly IAudioService _audioService;
    private readonly IFileService _fileService;
    private AudioModel? _audio;

    public string Title => Format(this._audio?.DisplayTitle);
    public string Artists => Format(this._audio?.Metadata.Artists);
    public string AlbumArtists => Format(this._audio?.Metadata.AlbumArtists);
    public string Composers => Format(this._audio?.Metadata.Composers);
    public string Genres => Format(this._audio?.Metadata.Genres);
    public string Group => Format(this._audio?.Metadata.Grouping);
    public string Album => Format(this._audio?.Metadata.Album);
    public string Comment => Format(this._audio?.Metadata.Comment);
    public string Lyrics => Format(this._audio?.Metadata.Lyrics);
    public string Track => Format(this._audio?.Metadata.Track, this._audio?.Metadata.TrackCount);
    public string Disc => Format(this._audio?.Metadata.Disc, this._audio?.Metadata.DiscCount);
    public string Duration => Format(this._audio?.Metadata.Duration);
    public IReadOnlyList<FileEntryViewModel> Files { get; private set; }
    public Hash256 FileHash => this._audio?.Hash ?? new Hash256();
    private double _fileWidthRequest = 0;
    public double FileWidthRequest {
        get => _fileWidthRequest;
        set {
            if (this._fileWidthRequest != value) {
                this._fileWidthRequest = value;
                OnPropertyChanged();
            }
        }
    }
    private static readonly string[] PropertyNames = [
        nameof(Title), nameof(Artists), nameof(AlbumArtists),
        nameof(Composers), nameof(Genres), nameof(Album),
        nameof(Comment), nameof(Lyrics), nameof(Track),
        nameof(Disc), nameof(Duration), nameof(Group),
        nameof(FileHash), nameof(Files)
        ];
    public static string Format(string? value) {
        return value ?? string.Empty;
    }
    public string Format(int? current, int? total) {
        var format = this.L["Info_Template_CurrentTotal"];
        return string.Format(format, current, total);
    }
    public static string Format(IReadOnlyList<string>? values) {
        if (values == null) {
            return string.Empty;
        } else {
            return string.Join(',', values);
        }
    }
    public string Format(TimeSpan? durationNullable) {
        if (durationNullable == null) {
            return "0";
        }
        var duration = durationNullable.Value;
        string format = this.L["Format_Time_HourMinuteSecond"];
        return string.Format(format, Math.Floor(duration.TotalHours), duration.Minutes.ToString("D2"), duration.Seconds.ToString("D2"));
    }
    public AudioInfoOverlayViewModel(ILocalizationService localizationService, IAudioService audioService, IFileService fileService): base(localizationService) {
        this._audioService = audioService;
        this._fileService = fileService;
        this.Files = [];
    }
    public async Task Initialize(Hash256 fileHash) {
        this._audio = await this._audioService.Get(fileHash);
        if (this._audio != null) {
            var fileEntries = await this._fileService.GetByHash(fileHash);
            List<FileEntryViewModel> files = [];
            foreach (var fileEntry in fileEntries) {
                FileEntryViewModel item = new(fileEntry.Hash, fileEntry.Path);
                files.Add(item);
            }
            this.Files = files;
            foreach(var propertyName in PropertyNames) {
                OnPropertyChanged(propertyName);
            }
        }
    }
}