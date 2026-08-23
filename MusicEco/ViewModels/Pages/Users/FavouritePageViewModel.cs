using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Users;

public partial class FavouritePageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Favourite;
    private readonly IFavouriteService _favouriteService;
    private readonly IPlaybackService _playbackService;
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleList)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public FavouritePageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IFavouriteService favouriteService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._favouriteService = favouriteService;
        this._playbackService = playbackService;
        this.Items = new();
    }

    private async void FavouriteService_ItemsChanged(object? sender, EventArgs e) {
        await this.Refresh();
    }

    public override async Task Refresh() {
        var entries = await this._favouriteService.GetFavourites();
        List<AudioEntryViewModel> items = [];
        foreach(var entry in entries) {
            AudioEntryViewModel item = new(entry.Hash, entry.Title);
            items.Add(item);
        }
        Items.Update(items);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        await Refresh();
        this._favouriteService.ItemsChanged += this.FavouriteService_ItemsChanged;
        FireNavigated(e);
    }
    public override async Task OnNavigatedFrom(NavigateEventArgs e) {
        await base.OnNavigatedFrom(e);
        this._favouriteService.ItemsChanged -= FavouriteService_ItemsChanged;
    }
    [RelayCommand]
    private async Task SelectItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        string queueName = L["Queue_Template_Favourite"];
        List<AudioEntry> audios = await this._favouriteService.GetFavourites();
        AudioEntry? current = null;
        foreach(var audio in audios) {
            if (audio.Hash == vm.FileHash) {
                current = audio;
                break;
            }
        }
        if (current != null) {
            await this._playbackService.PlayQueue(queueName, audios, current, this);
        }
    }
}
