using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Users;

public partial class NotPlayPageViewModel: BasePageViewModel {
    private sealed class Query {
        public string Name;
        public Query(string name) {
            this.Name = name;
        }
    }
    public override PageRoute Route => PageRoute.NotPlay;
    private readonly Query _q;
    private readonly IAudioQueryService _queryService;
    private readonly IPlaybackService _playbackService;
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    public NotPlayPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAudioQueryService audioQueryService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._queryService = audioQueryService;
        this._playbackService = playbackService;
        this._q = new(string.Empty);
        this.Items = new();
    }
    public override async Task Refresh() {
        var audios = await this._queryService.GetNotPlay(Config.MinPlayedRatio, this._q.Name);
        List<AudioEntryViewModel> items = [];
        foreach (var audio in audios) {
            AudioEntryViewModel item = new(audio.Hash, audio.Title);
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
        string queueName = this.L["Queue_Template_NotPlayed"];
        AudioEntry? current = null;
        List<AudioEntry> audios = [];
        foreach(var item in this.Items.Items) {
            AudioEntry audio = new(item.FileHash, item.DisplayTitle);
            if (audio.Hash == vm.FileHash) {
                current = audio;
            }
            audios.Add(audio);
        }
        if (current != null) {
            await this._playbackService.PlayQueue(queueName, audios, current, this);
        }
    }
}
