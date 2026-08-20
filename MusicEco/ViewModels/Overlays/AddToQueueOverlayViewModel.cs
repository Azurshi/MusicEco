using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;
using MusicEco.ViewModels.Pages;

namespace MusicEco.ViewModels.Overlays;

public partial class AddToQueueOverlayViewModel: BaseOverlayViewModel {
    private readonly IQueueService _queueService;
    // Weak Action to prevent ViewModel keep View alive
    private WeakReference<Action>? _closeRef;
    public IReadOnlyList<QueueItemViewModel> Items { get; private set; }
    private Hash256? _selectedHash;
    private readonly Dictionary<DateTime, bool> _canSelectMap;
    private bool _initialized = false;
    public AddToQueueOverlayViewModel(ILocalizationService localizationService, IQueueService queueService): base(localizationService) {
        this._queueService = queueService;
        this.Items = [];
        this._canSelectMap = [];
    }
    public async Task Initialize(Hash256 fileHash, Action close) {
        this._closeRef = new(close);
        this._selectedHash = fileHash;
        var queues = await this._queueService.GetAll();
        List<QueueItemViewModel> items = [];
        foreach (var queue in queues.OrderBy(q => q.LastPlayTime)) {
            QueueItemViewModel item = new(queue.CreationTime, queue.ModifiedTime, queue.LastPlayTime, queue.Name);
            items.Add(item);
            bool contain = queue.Audios.Select(a => a.Hash).Contains(fileHash);
            this._canSelectMap[queue.CreationTime] = !contain;
        }
        this.Items = items;
        OnPropertyChanged(nameof(Items));
        this._initialized = true;
        SelectItemCommand.NotifyCanExecute();
    }
    private bool CanSelectItem(QueueItemViewModel? vm) {
        if (!this._initialized || vm == null || this._selectedHash == null) {
            return false;
        }
        return this._canSelectMap[vm.CreationTime];
    }
    [RelayCommand(CanExecute = nameof(CanSelectItem))]
    private async Task SelectItem(QueueItemViewModel? vm) {
        if (!this._initialized || vm == null || this._selectedHash == null) {
            return;
        }
        var queue = await this._queueService.Get(vm.CreationTime);
        if (queue != null) {
            var audios = queue.Audios.Append(new(this._selectedHash.Value, string.Empty)).ToList();
            queue = queue.WithAudios(queue.Current, audios).WithModifyNow();
            await this._queueService.Update(queue, this);
            if (this._closeRef!.TryGetTarget(out var target)) {
                target.Invoke();
            } else {
                throw new InvalidOperationException("View close Action already collected by GC");
            }
        }
    }
}
