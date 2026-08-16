using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Users;

public partial class PlayHistoryPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.PlayHistory;
    private readonly IAppSetting _setting;
    private readonly IAudioQueryService _queryService;
    private readonly IPlaybackService _playbackService;
    private readonly IPlayerController _playerController;
    public ObservableCollectionExtend<PlayHistoryViewModel> Items { get; init; }
    public AsyncCommand<PlayHistoryViewModel> SelectItemCommand { get; init; }
    public PlayHistoryPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAudioQueryService audioQueryService, IPlaybackService playbackService, IPlayerController playerController) : base(localizationService) {
        this._setting = appSetting;
        this._queryService = audioQueryService;
        this._playbackService = playbackService;
        this._playerController = playerController;
        this.Items = new();
        this.SelectItemCommand = new(SelectItem);
        AppLifeCycle.RegisterLoop("PlayHistory", (provider) => {
            if (this.IsActive) {
                foreach (var item in this.Items.Items) {
                    item.RefreshNotify();
                }
            }
        }, TimeSpan.FromSeconds(1));
        this._playerController.TrackChanged += this.PlayerController_TrackChanged;
    }

    private async void PlayerController_TrackChanged(object? sender, TrackChangedEventArgs e) {
        if (this.IsActive) {
            if (e.Reason == TrackChangeReason.Initialize) {
                return;
            }
            else {
                await Refresh();
            }
        }
    }

    public override async Task Refresh() {
        var histories = await this._queryService.GetPlayHistory(Config.MinPlayedRatio);
        List<PlayHistoryViewModel> items = [];
        foreach(var history in histories) {
            PlayHistoryViewModel item = new(history.Audio.Hash, history.Audio.Title, history.LastPlayTime);
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
    private async Task SelectItem(PlayHistoryViewModel? vm) {
        if (vm == null) {
            return;
        }
        string queueName = this.L["Queue_Template_PlayHistory"];
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
