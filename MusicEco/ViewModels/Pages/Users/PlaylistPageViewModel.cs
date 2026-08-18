using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Users;

public sealed partial class PlaylistPageQuery: ObservableObject {
    [ObservableProperty]
    public partial string Name { get; set; }
    public PlaylistPageQuery() {
        this.Name = string.Empty;
    }
}

public partial class PlaylistPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Playlist;
    private readonly IPlaylistService _playlistService;
    public PlaylistPageQuery Query { get; init; }
    private readonly DelayedDispatcher _queryDispatcher;
    public ManagedCollection<PlaylistItemViewModel> Items { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleList)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public PlaylistPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IPlaylistService playlistService) : base(localizationService, appSetting) {
        this.Query = new();
        this._queryDispatcher = new(Config.UserInputDelay);
        this._playlistService = playlistService;
        this.Items = new(this.Filter);
        this.Query.PropertyChanged += this.Query_PropertyChanged;
    }

    private async void Query_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
        await this._queryDispatcher.Dispatch(this.Items.Refresh);
    }

    private async void PlaylistService_ItemsChanged(object? sender, PlaylistChangedEventArgs e) {
        await Refresh();
    }
    private IReadOnlyList<PlaylistItemViewModel> Filter(IReadOnlyList<PlaylistItemViewModel> items) {
        string nameQuery = this.Query.Name.Trim();
        if (nameQuery.Length >= Config.MinNameLength) {
            List<PlaylistItemViewModel> result = [];
            foreach (var item in items) {
                if (item.Name.Contains(nameQuery, StringComparison.InvariantCultureIgnoreCase)) {
                    result.Add(item);
                }
            }
            return result;
        }
        else {
            return items;
        }
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
    [RelayCommand]
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
