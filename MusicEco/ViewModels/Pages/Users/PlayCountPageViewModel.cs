using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Users;

public partial class PlayCountPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.PlayCount;
    private readonly IAudioQueryService _queryService;
    private readonly IPlaybackService _playbackService;
    public ObservableCollectionExtend<PlayCountViewModel> Items { get; init; }
    public PlayCountPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAudioQueryService audioQueryService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._queryService = audioQueryService;
        this._playbackService = playbackService;
        this.Items = new();
        
    }
    public override async Task Refresh() {
        var playCounts = await this._queryService.QueryPlayCount(Config.MinPlayedRatio, DateTime.MinValue, DateTime.MaxValue);
        List<PlayCountViewModel> items = [];
        foreach (var playCount in playCounts) {
            PlayCountViewModel item = new(playCount.Audio.Hash, playCount.Audio.Title, playCount.PlayCount);
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
    private async Task SelectItem(PlayCountViewModel? vm) {
        if (vm == null) {
            return;
        }
        string format = this.L["Queue_Template_PlayCount"];
        string queueName = string.Format(format, "Test");
        AudioEntry? current = null;
        List<AudioEntry> audios = [];
        foreach (var item in this.Items.Items) {
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
