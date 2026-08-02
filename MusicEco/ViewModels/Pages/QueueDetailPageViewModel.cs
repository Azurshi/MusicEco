using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class QueueDetailPageViewModel: BasePageViewModel {
    private sealed class Query {
        public DateTime CreationTime;
        public Query(DateTime creationTime) {
            this.CreationTime = creationTime;
        }
    }

    public override PageRoute Route => PageRoute.QueueDetail;
    private readonly IQueueService _queueService;
    private readonly IPlaybackService _playbackService;
    private readonly Query _q;
    public string QueueName { get; private set; }
    public ObservableCollectionExtend<AudioEntryViewModel> Items { get; init; }
    public AsyncCommandExtend<AudioEntryViewModel> SelectItemCommand { get; init; }
    public QueueDetailPageViewModel(ILocalizationService localizationService, IQueueService queueService, IPlaybackService playbackService) : base(localizationService) {
        this._queueService = queueService;
        this._playbackService = playbackService;
        this._q = new(DateTime.MaxValue);
        this.Items = new();
        this.QueueName = string.Empty;
        this.SelectItemCommand = new(SelectItem, CanSelectItem);
    }
    public override async Task Refresh() {
        var audioQueue = await this._queueService.Get(_q.CreationTime);
        List<AudioEntryViewModel> items = [];
        if (audioQueue != null) {
            this.QueueName = audioQueue.Name;
            foreach (var audio in audioQueue.Audios) {
                AudioEntryViewModel item = new(audio.Hash, audio.Title);
                items.Add(item);
            }
            OnPropertyChanged(nameof(QueueName));
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
            //await this._queueService.Update(audioQueue, this);
            await this._playbackService.PlayQueue(audioQueue, this);
        }
    }
}
