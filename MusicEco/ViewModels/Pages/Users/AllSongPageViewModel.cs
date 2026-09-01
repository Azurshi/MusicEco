using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Users;

public partial class AllSongPageViewModel: BasePageViewModel {
    private sealed class Query {
        public string Name;
        public Query(string name) {
            this.Name = name;
        }
    }
    public override PageRoute Route => PageRoute.AllSong;
    private readonly Query _q;
    private readonly IAudioService _audioService;
    private readonly IPlaybackService _playbackService;
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleList)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public AllSongPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAudioService audioService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._audioService = audioService;
        this._playbackService = playbackService;
        this._q = new(string.Empty);
        this.Items = new();
    }
    public override async Task Refresh() {
        string nameLike = string.Empty;
        if (this._q.Name.Length >= Config.MinNameLength) {
            nameLike = this._q.Name;
        }
        var entries = await this._audioService.QueryEntry(nameLike);
        List<AudioEntryViewModel> items = [];
        foreach(var entry in entries) {
            AudioEntryViewModel item = new(entry.Hash, entry.DisplayTitle);
            items.Add(item);
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
    private async Task SelectItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        var queueName = this.L["Queue_Template_All"];
        AudioEntry? current = null;
        List<AudioEntry> audios = [];
        foreach(var item in this.Items.Items) {
            AudioEntry entry = new(item.FileHash, item.DisplayTitle);
            if (entry.Hash == vm.FileHash) {
                current = entry;
            }
            audios.Add(entry);
        }
        if (current != null) {
            await this._playbackService.PlayQueue(queueName, audios, current, this);
        }
    }
}
