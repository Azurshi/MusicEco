using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class AlbumPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Album;
    private readonly string _nameQuery;
    private readonly IAudioQueryService _queryService;
    public ObservableCollectionExtend<AlbumViewModel> Items { get; init; }
    //public SyncCommand<AlbumViewModel> SelectItemCommand { get; init; }
    public CollectionDisplayMode DisplayMode {
        get => this._setting.Get(CollectionDisplayMode.SimpleGrid, $"Album.{nameof(DisplayMode)}");
        set {
            this._setting.Set(value, $"Album.{nameof(DisplayMode)}");
            OnPropertyChanged();
        }
    }
    public AlbumPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAudioQueryService audioQueryService) : base(localizationService, appSetting) {
        this.Items = new();
        this._nameQuery = string.Empty;
        this._queryService = audioQueryService;
        //this.SelectItemCommand = new(SelectItem);
        //this.SelectItemCommand
    }
    public override async Task Refresh() {
        string query = this._nameQuery;
        var albums = await this._queryService.QueryAlbum(query);
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
