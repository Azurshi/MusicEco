using MusicEco.Core;
using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class SearchPageQuery: ObservableObject {
    [ObservableProperty]
    public partial string SearchText { get; set; }
    public SearchPageQuery() {
        this.SearchText = string.Empty;
    }
}

public partial class SearchPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Search;
    public SearchPageQuery Query { get; init; }
    private readonly IAudioService _audioService;
    private readonly IPlaybackService _playbackService;
    private DelayedDispatcher? _dispatcher;
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    public SearchPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAudioService audioService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this.Query = new();
        this._dispatcher = null;
        this._audioService = audioService;
        this._playbackService = playbackService;
        this.Items = new();
        this._cachedAction = new(() => this.Refresh().FireAndForgetAsync());
        this.Query.PropertyChanged += this.Query_PropertyChanged;
    }
    public void InitDispatcher(IDispatcher dispatcher) {
        this._dispatcher = new(dispatcher, Config.UserInputDelay);
    }
    private readonly Action _cachedAction;
    private async void Query_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
        this._dispatcher?.Dispatch(this._cachedAction);
    }
    public override async Task Refresh() {
        bool valid = this.Query.SearchText.Trim().Length >= Config.MinNameLength;
        List<AudioEntryViewModel> items = [];
        if (valid) {
            string query = this.Query.SearchText;
            var entries = await this._audioService.QueryEntry(query);
            foreach (var entry in entries) {
                items.Add(new(entry.Hash, entry.DisplayTitle));
            }
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
        string format = this.L["Queue_Template_Search"];
        string queueName = string.Format(format, this.Query.SearchText.Trim());
        AudioEntry? current = null;
        List<AudioEntry> audios = [];
        foreach (var item in this.Items.Items) {
            AudioEntry entry = new(item.FileHash, item.DisplayTitle);
            audios.Add(entry);
            if (item.FileHash == vm.FileHash) {
                current = entry;
            }
        }
        if (current != null) {
            await this._playbackService.PlayQueue(queueName, audios, current, this);
        }
    }
}
