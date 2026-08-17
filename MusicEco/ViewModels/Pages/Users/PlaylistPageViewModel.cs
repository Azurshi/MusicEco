using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Users;

public partial class PlaylistPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Playlist;
    private readonly IPlaylistService _playlistService;
    public ObservableCollectionExtend<PlaylistItemViewModel> Items { get; init; }
    public AsyncCommand<PlaylistItemViewModel> SelectItemCommad { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleList)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public PlaylistPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IPlaylistService playlistService) : base(localizationService, appSetting) {
        this._playlistService = playlistService;
        this.Items = new();
        this.SelectItemCommad = new(SelectItem);
    }

    private async void PlaylistService_ItemsChanged(object? sender, PlaylistChangedEventArgs e) {
        await Refresh();
    }

    public override async Task Refresh() {
        var playlists = await this._playlistService.GetAll();
        List<PlaylistItemViewModel> items = [];
        foreach(var playlist in playlists) {
            PlaylistItemViewModel item = new(playlist.CreationTime, playlist.ModifiedTime, playlist.Name);
            items.Add(item);
        }
        Items.Update(items);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        await Refresh();
        FireNavigated(e);
        this._playlistService.ItemsChanged += this.PlaylistService_ItemsChanged;
    }
    public override async Task OnNavigatedFrom(NavigateEventArgs e) {
        await base.OnNavigatedFrom(e);
        this._playlistService.ItemsChanged -= this.PlaylistService_ItemsChanged;
    }
    private async Task SelectItem(PlaylistItemViewModel? vm) {
        if (vm == null) {
            return;
        }
        Dictionary<string, object> query = new() {
            ["creationTime"] = vm.CreationTime
        };
        NavigateEventArgs args = new(this, this.Route, PageRoute.PlaylistDetail, query);
        EventSystem.Publish(this, args);
    }
}
