using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class AlbumPageQuery: ObservableObject {
    [ObservableProperty]
    public partial string Name { get; set; }
    public AlbumPageQuery() {
        this.Name = string.Empty;
    }
}
public partial class AlbumPageViewModel: BasePageViewModel {

    public override PageRoute Route => PageRoute.Album;
    public AlbumPageQuery Query { get; init; } = new();
    private readonly DelayedDispatcherEx _queryDispatcher;
    private readonly IAudioQueryService _queryService;
    public ManagedCollection<AlbumViewModel> Items { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleGrid)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public AlbumPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAudioQueryService audioQueryService) : base(localizationService, appSetting) {
        this.Items = new(this.Filter);
        this._queryService = audioQueryService;
        this.Query.PropertyChanged += this.Query_PropertyChanged;
        this._queryDispatcher = new(Config.UserInputDelay);
    }
    private IReadOnlyList<AlbumViewModel> Filter(IReadOnlyList<AlbumViewModel> items) {
        string nameQuery = this.Query.Name.Trim();
        if (nameQuery.Length >= Config.MinNameLength) {
            List<AlbumViewModel> result = [];
            foreach (var item in items) {
                if (item.Name.Contains(nameQuery, StringComparison.InvariantCultureIgnoreCase)) {
                    result.Add(item);
                }
            }
            return result;
        } else {
            return items;
        }
    }
    private async void Query_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
        await this._queryDispatcher.Dispatch(this.Items.Refresh);
    }
    public override async Task Refresh() {
        var albums = await this._queryService.GetAlbums();
        List<AlbumViewModel> items = [];
        foreach (var album in albums) {
            items.Add(new(album.Name, album.Audios.Select(a => a.Hash).ToList()));
        }
        this.Items.Update(items);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        await Refresh();
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
    [RelayCommand]
    private void SelectItem(AlbumViewModel? vm) {
        if (vm == null) {
            return;
        }
        Dictionary<string, object> query = new() {
            ["albumName"] = vm.Name
        };
        NavigateEventArgs args = new(this, this.Route, PageRoute.AlbumDetail, query);
        EventSystem.Publish(this, args);
    }
}
