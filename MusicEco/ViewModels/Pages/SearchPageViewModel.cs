using MusicEco.Core;
using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class SearchPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Search;
    private readonly IAudioService _audioService;
    private string _searchText = string.Empty;
    public string SearchText {
        get => _searchText;
        set {
            if (this._searchText != value) {
                this._searchText = value;
                OnPropertyChanged();
                if (this._searchText.Trim().Length > 2) {
                    Refresh().FireAndForgetAsync();
                }
            }
        }
    }
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    public SearchPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAudioService audioService) : base(localizationService, appSetting) {
        this._audioService = audioService;
        this.Items = new();
    }
    public override async Task Refresh() {
        if (this._searchText.Trim().Length < 3) {
            return;
        }
        string query = this._searchText;
        var entries = await this._audioService.QueryEntry(query);
        List<AudioEntryViewModel> items = [];
        foreach(var entry in entries) {
            items.Add(new(entry.Hash, entry.Title));
        }
        this.Items.Update(items);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
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
        var queueService = AppLifeCycle.Provider.GetRequiredService<IQueueService>();
        string queueName = $"Search {_searchText.Trim()}";
        bool exists = await queueService.Exists(queueName);
        if (!exists) {
            AudioEntry? currentEntry = null;
            List<AudioEntry> entries = [];
            foreach(var item in this.Items.Items) {
                AudioEntry entry = new(item.FileHash, item.DisplayTitle);
                entries.Add(entry);
                if (item.FileHash == vm.FileHash) {
                    currentEntry = entry;
                }
            }
            AudioQueue queue = new(DateTime.Now, queueName, DateTime.Now, DateTime.Now, currentEntry, entries);
            await queueService.Insert(queue, this);
        }
    }
}
