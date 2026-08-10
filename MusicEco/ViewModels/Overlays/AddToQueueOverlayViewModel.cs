using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;
using MusicEco.ViewModels.Items;
using MusicEco.ViewModels.Pages;

namespace MusicEco.ViewModels.Overlays;

public partial class AddToQueueOverlayViewModel: ObservableObject {
    private readonly IQueueService _queueService;
    private WeakReference<Action>? _closeRef;
    public AssemblyLocalization L { get; init; }
    public IReadOnlyList<QueueItemViewModel> Items { get; private set; }
    public AsyncCommandExtend<QueueItemViewModel> SelectItemCommand { get; init; }
    private Hash256? _selectedHash;
    private readonly Dictionary<DateTime, bool> _canSelectMap;
    private bool _initialized = false;
    public AddToQueueOverlayViewModel(IQueueService queueService, ILocalizationService localizationService) {
        this.L = localizationService.Get(typeof(BasePageViewModel));
        this._queueService = queueService;
        this.Items = [];
        this._canSelectMap = [];
        this.SelectItemCommand = new(SelectItem, CanSelectItem);
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
        Items = items;
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
    private async Task SelectItem(QueueItemViewModel? vm) {
        if (!this._initialized || vm == null || this._selectedHash == null) {
            return;
        }
        var queue = await this._queueService.Get(vm.CreationTime);
        if (queue != null) {
            var audios = queue.Audios.Append(new(this._selectedHash.Value, string.Empty)).ToList();
            queue = queue.WithAudios(queue.Current, audios);
            await this._queueService.Update(queue, this);
            if (this._closeRef!.TryGetTarget(out var target)) {
                target.Invoke();
            } else {
                throw new Exception();
            }
        }
    }
}
