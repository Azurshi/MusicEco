using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Pages;
using System.Diagnostics;

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
    private readonly IPlaylistService _playlistService;
    private readonly IPlaybackService _playbackService;
    public string PlaylistName { get; private set; }
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    public AsyncCommand<AudioEntryViewModel> SelectItemCommand { get; init; }
    public AsyncCommand<AudioEntryViewModel> RemoveItemCommand { get; init; }
    public SyncCommandExtend<AudioEntryViewModel> DragCommand { get; init; }
    public AsyncCommand<AudioEntryViewModel> DropCommand { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleList)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public PlaylistDetailPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IPlaylistService playlistService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._q = new(DateTime.MaxValue);
        this._playlistService = playlistService;
        this._playbackService = playbackService;
        this.PlaylistName = string.Empty;
        this.Items = new();
        this.SelectItemCommand = new(SelectItem);
        this.RemoveItemCommand = new(RemoveItem);
        this.DragCommand = new(OnItemDrag, IsDraggable);
        this.DropCommand = new(OnItemDrop);
    }

    private async void PlaylistService_ItemsChanged(object? sender, PlaylistChangedEventArgs e) {
        if (e.CreationTime == this._q.CreationTime) {
            await Refresh();
        }
    }

    public override async Task Refresh() {
        var audioPlaylist = await this._playlistService.Get(this._q.CreationTime);
        List<AudioEntryViewModel> items = [];
        if (audioPlaylist != null) {
            this.PlaylistName = audioPlaylist.Name;
            foreach (var audio in audioPlaylist.Audios) {
                AudioEntryViewModel item = new(audio.Hash, audio.Title);
                items.Add(item);
            }
        }
        else {
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
        this._playlistService.ItemsChanged += this.PlaylistService_ItemsChanged;
        FireNavigated(e);
    }
    public override async Task OnNavigatedFrom(NavigateEventArgs e) {
        await base.OnNavigatedFrom(e);
        this._playlistService.ItemsChanged -= this.PlaylistService_ItemsChanged;
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
            foreach (var entry in audioPlaylist.Audios) {
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
            foreach (var audio in originalAudios) {
                if (audio.Hash != vm.FileHash) {
                    modifiedAudios.Add(audio);
                }
            }
            audioPlaylist = audioPlaylist.WithAudios(modifiedAudios).WithModifyNow();
            await this._playlistService.Update(audioPlaylist);
        }
    }
    #region Drag&Drop
    private AudioEntryViewModel? _movingItem;
    private void OnItemDrag(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        this._movingItem = vm;
    }
    private async Task OnItemDrop(AudioEntryViewModel? vm) {
        if (vm == null || this._movingItem == null) {
            return;
        }
        int targetIndex = this.Items.Items.IndexOf(vm);
        int currentIndex = this.Items.Items.IndexOf(this._movingItem);
        Debug.WriteLine($"Moving: {currentIndex} -> {targetIndex}");
        if (targetIndex == 0 || currentIndex == targetIndex) {
            return;
        }
        // For data
        var playlist = await this._playlistService.Get(this._q.CreationTime);
        if (playlist != null) {
            var audios = playlist.Audios.ToList();
            var current = audios[currentIndex];
            audios.RemoveAt(currentIndex);
            audios.Insert(targetIndex, current);
            playlist = playlist.WithAudios(audios).WithModifyNow();
            await this._playlistService.Update(playlist);
        }
    }
    private bool IsDraggable(AudioEntryViewModel? vm) {
        if (vm != null && vm.IsDraggable) {
            return true;
        } else {
            return false;
        }
    }
    #endregion
}
