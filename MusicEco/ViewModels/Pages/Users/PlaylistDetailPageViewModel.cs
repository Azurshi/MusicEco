using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Pages;

namespace MusicEco.ViewModels.Pages.Users;

public partial class PlaylistDetailPageViewModel: BasePageViewModel {
    private sealed class Query {
        public DateTime CreationTime;
        public Query(DateTime creationTime) {
            this.CreationTime = creationTime;
        }
    }
    public override PageRoute Route => PageRoute.PlaylistDetail;
    private readonly Query _q;
    private readonly IAppSetting _setting;
    private readonly IPlaylistService _playlistService;
    private readonly IPlaybackService _playbackService;
    public string PlaylistName { get; private set; }
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    public AsyncCommand<AudioEntryViewModel> SelectItemCommand { get; init; }
    public AsyncCommand<AudioEntryViewModel> RemoveItemCommand { get; init; }
    public CollectionDisplayMode DisplayMode {
        get => this._setting.Get(CollectionDisplayMode.SimpleList, $"PlaylistDetail.{nameof(DisplayMode)}");
        set {
            this._setting.Set(value, $"PlaylistDetail.{nameof(DisplayMode)}");
            OnPropertyChanged();
        }
    }
    public PlaylistDetailPageViewModel(ILocalizationService localizationService, IAppSetting setting, IPlaylistService playlistService, IPlaybackService playbackService) : base(localizationService) {
        this._q = new(DateTime.MaxValue);
        this._setting = setting;
        this._playlistService = playlistService;
        this._playbackService = playbackService;
        this.PlaylistName = string.Empty;
        this.Items = new();
        this.SelectItemCommand = new(SelectItem);
        this.RemoveItemCommand = new(RemoveItem);
    }
    public override async Task Refresh() {
        var audioPlaylist = await this._playlistService.Get(this._q.CreationTime);
        List<AudioEntryViewModel> items = [];
        if (audioPlaylist != null) {
            this.PlaylistName = audioPlaylist.Name;
            foreach(var audio in audioPlaylist.Audios) {
                AudioEntryViewModel item = new(audio.Hash, audio.Title);
                items.Add(item);
            }
        } else {
            this.PlaylistName = string.Empty;
        }
        OnPropertyChanged(nameof(PlaylistName));
        Items.Update(items);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        if (e.Query.TryGetValue("creationTime", out var creationTimeObj)) {
            if (creationTimeObj is DateTime creaionTime) {
                this._q.CreationTime = creaionTime;
                await Refresh();
            }
        }
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
    private async Task SelectItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        var audioPlaylist = await this._playlistService.Get(this._q.CreationTime);
        if (audioPlaylist != null) {
            string format = this.L["Queue_Template_Playlist"];
            string queueName = string.Format(format, audioPlaylist.Name);
            AudioEntry? current = null;
            foreach(var entry in audioPlaylist.Audios) {
                if (entry.Hash == vm.FileHash) {
                    current = entry;
                    break;
                }
            }
            if (current != null) {
                audioPlaylist = audioPlaylist.WithPlayNow();
                var success = await this._playlistService.Update(audioPlaylist);
                if (success) {
                    await this._playbackService.PlayQueue(queueName, audioPlaylist.Audios.ToList(), current, this);
                }
            }
        }
    }
    private async Task RemoveItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        var audioPlaylist = await this._playlistService.Get(this._q.CreationTime);
        if (audioPlaylist != null) {
            var originalAudios = audioPlaylist.Audios;
            List<AudioEntry> modifiedAudios = [];
            foreach(var audio in originalAudios) {
                if (audio.Hash != vm.FileHash) {
                    modifiedAudios.Add(audio);
                }
            }
            audioPlaylist = audioPlaylist.WithAudios(modifiedAudios).WithModifyNow();
            await this._playlistService.Update(audioPlaylist);
        }
    }
}
