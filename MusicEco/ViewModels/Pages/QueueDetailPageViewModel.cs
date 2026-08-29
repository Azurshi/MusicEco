using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;
using System.Diagnostics;

namespace MusicEco.ViewModels.Pages;

public partial class QueueDetailPageViewModel: BasePageViewModel {
    private sealed class Query {
        public DateTime CreationTime;
        public Query(DateTime creationTime) {
            this.CreationTime = creationTime;
        }
    }

    public override PageRoute Route => PageRoute.QueueDetail;
    private readonly Query _q;
    private readonly IQueueService _queueService;
    private readonly IPlaybackService _playbackService;
    public string QueueName { get; private set; }
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    public QueueDetailPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IQueueService queueService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._queueService = queueService;
        this._playbackService = playbackService;
        this._q = new(DateTime.MaxValue);
        this.Items = new();
        this.QueueName = string.Empty;
    }
    public override async Task Refresh() {
        var audioQueue = await this._queueService.Get(_q.CreationTime);
        List<AudioEntryViewModel> items = [];
        if (audioQueue != null) {
            this.QueueName = audioQueue.Name;
            OnPropertyChanged(nameof(QueueName));
            foreach (var audio in audioQueue.Audios) {
                AudioEntryViewModel item = new(audio.Hash, audio.DisplayTitle);
                items.Add(item);
            }
            var current = audioQueue.Current;
            this.Items.Update(items, (item) => {
                bool isCurrent = false;
                if (current != null && current.Hash == item.FileHash) {
                    isCurrent = true;
                }
                item.Selected = isCurrent;
            });
        }
        else {
            this.QueueName = string.Empty;
            OnPropertyChanged(nameof(QueueName));
            this.Items.Update(items);
        }

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
        this._queueService.ItemsChanged += this.QueueService_ItemsChanged;
    }

    private async void QueueService_ItemsChanged(object? sender, QueueChangedEventArgs e) {
        if (e.Kind == ChangeKind.Updated) {
            await Refresh();
        }
    }

    public override async Task OnNavigatedFrom(NavigateEventArgs e) {
        await base.OnNavigatedFrom(e);
        this._queueService.ItemsChanged -= this.QueueService_ItemsChanged;
    }
    private bool CanSelectItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return false;
        }
        return true;
    }
    [RelayCommand(CanExecute = nameof(CanSelectItem))]
    private async Task SelectItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        var audioQueue = await this._queueService.Get(_q.CreationTime);
        if (audioQueue != null) {
            AudioEntry? current = null;
            foreach(var audio in audioQueue.Audios) {
                if (audio.Hash == vm.FileHash) {
                    current = audio;
                }
            }
            audioQueue = audioQueue.WithCurrent(current).WithModifyNow();
            await this._playbackService.PlayQueue(audioQueue, this);
        }
    }
    [RelayCommand]
    private async Task RemoveItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        var audioQueue = await this._queueService.Get(_q.CreationTime);
        if (audioQueue != null) {
            int targetIndex = -1;
            for(int i=0; i<audioQueue.Audios.Count; i++) {
                var audio = audioQueue.Audios[i];
                if (audio.Hash == vm.FileHash) {
                    targetIndex = i;
                    break;
                }
            }
            var audios = audioQueue.Audios.ToList();
            audios.RemoveAt(targetIndex);
            AudioEntry? current;
            if (audioQueue.Current?.Hash == vm.FileHash) {
                current = null;
            } else {
                current = audioQueue.Current;
            }
            audioQueue = audioQueue.WithAudios(current, audios).WithModifyNow();
            await this._queueService.Update(audioQueue, this);
        }
    }
    #region Drag&Drop
    private AudioEntryViewModel? _movingItem;
    [RelayCommand(CanExecute = nameof(IsDraggable))]
    private void DragItem(AudioEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        this._movingItem = vm;
    }
    [RelayCommand]
    private async Task DropItem(AudioEntryViewModel? vm) {
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
        var queue = await this._queueService.Get(this._q.CreationTime);
        if (queue != null) {
            var audios = queue.Audios.ToList();
            var current = audios[currentIndex];
            audios.RemoveAt(currentIndex);
            audios.Insert(targetIndex, current);
            queue = queue.WithAudios(queue.Current, audios).WithModifyNow();
            await this._queueService.Update(queue, this);
        }
    }
    private bool IsDraggable(AudioEntryViewModel? vm) {
        if (vm != null && vm.IsDraggable) {
            return true;
        }
        else {
            return false;
        }
    }
    #endregion
}
