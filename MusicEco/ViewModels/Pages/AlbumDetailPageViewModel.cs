using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class AlbumDetailPageViewModel: BasePageViewModel {
    private sealed class Query {
        public string AlbumName;
        public Query() {
            this.AlbumName = string.Empty;
        }
    }
    public override PageRoute Route => PageRoute.AlbumDetail;
    private readonly Query _q;
    private readonly IAudioQueryService _queryService;
    private readonly IPlaybackService _playbackService;
    public string AlbumName => _q.AlbumName;
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    public AsyncCommand<AudioEntryViewModel> SelectItemCommand { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleList)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public AlbumDetailPageViewModel(ILocalizationService localizationService, IAudioQueryService audioQueryService, IAppSetting appSetting, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._q = new();
        this._queryService = audioQueryService;
        this._playbackService = playbackService;
        this.Items = new();
        this.SelectItemCommand = new(SelectItem);

    }
    public override async Task Refresh() {
        OnPropertyChanged(nameof(AlbumName));
        var album = await this._queryService.GetAlbum(this._q.AlbumName);
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
                this._q.AlbumName = albumName;
                await Refresh();
            }
        }
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
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
