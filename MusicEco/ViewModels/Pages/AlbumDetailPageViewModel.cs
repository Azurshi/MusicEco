using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;
public partial class AlbumDetailPageQuery: ObservableObject {
    [ObservableProperty]
    public partial string AlbumName { get; set; }
    public AlbumDetailPageQuery() {
        this.AlbumName = string.Empty;
    }
}

public partial class AlbumDetailPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.AlbumDetail;
    public AlbumDetailPageQuery Query { get; init; }
    private readonly IAudioQueryService _queryService;
    private readonly IPlaybackService _playbackService;
    public string AlbumName => this.Query.AlbumName;
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleList)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public AlbumDetailPageViewModel(ILocalizationService localizationService, IAudioQueryService audioQueryService, IAppSetting appSetting, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this.Query = new();
        this._queryService = audioQueryService;
        this._playbackService = playbackService;
        this.Items = new();
    }

    public override async Task Refresh() {
        OnPropertyChanged(nameof(AlbumName));
        var album = await this._queryService.GetAlbum(this.Query.AlbumName);
        if (album != null) {
            List<AudioEntryViewModel> items = [];
            foreach(var audio in album.Audios) {
                AudioEntryViewModel item = new(audio.Hash, audio.Title);
                items.Add(item);
            }
            this.Items.Update(items);
        }
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        if (e.Query.TryGetValue("albumName", out var albumNameObj)) {
            if (albumNameObj is string albumName) {
                this.Query.AlbumName = albumName;
                await Refresh();
            }
        }
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
    [RelayCommand]
    private async Task SelectItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        string queueName = $"Album {AlbumName}";
        AudioEntry? selected = null;
        List<AudioEntry> audios = [];
        foreach(var item in this.Items.Items) {
            AudioEntry entry = new(item.FileHash, item.DisplayTitle);
            if (item == vm) {
                selected = entry;
            }
            audios.Add(entry);
        }
        if (selected == null) {
            throw new InvalidOperationException();
        }
        await this._playbackService.PlayQueue(queueName, audios, selected, this);
    }
}
