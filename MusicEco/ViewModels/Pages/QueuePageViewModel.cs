using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class QueuePageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Queue;
    private readonly IQueueService _queueService;
    public ObservableCollectionExtend<QueueItemViewModel> Items { get; init; }
    public AsyncCommand<QueueItemViewModel> SelectItemCommand { get; init; }
    public QueuePageViewModel(ILocalizationService localizationService, IQueueService queueService) : base(localizationService) {
        this._queueService = queueService;
        this.Items = new();
        this.SelectItemCommand = new(SelectItem);
    }
    public override async Task Refresh() {
        var queues = await _queueService.GetAll();
        var currentQueue = await _queueService.GetCurrent();
        List<QueueItemViewModel> items = [];
        foreach(var q in queues) {
            QueueItemViewModel item = new(q.CreationTime, q.ModifiedTime, q.Name);
            items.Add(item);
        }
        this.Items.Update(items, (item) => {
            bool isCurrent = false;
            if (currentQueue != null && currentQueue.CreationTime == item.CreationTime) {
                isCurrent = true;
            }
            item.Selected = isCurrent;
        });
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        FireNavigated(e);
        this._queueService.ItemsChanged += this.QueueService_ItemsChanged;
        await this.Refresh();
    }

    private async void QueueService_ItemsChanged(object? sender, QueueChangedEventArgs e) {
        if (e.Kind == ChangeKind.Added || e.Kind == ChangeKind.Removed || e.Kind == ChangeKind.Updated || e.Kind == ChangeKind.AllUpdated) {
            await this.Refresh();
        }
    }

    public override async Task OnNavigatedFrom(NavigateEventArgs e) {
        await base.OnNavigatedFrom(e);
        this._queueService.ItemsChanged -= this.QueueService_ItemsChanged;
    }

    private async Task SelectItem(QueueItemViewModel? vm) {
        if (vm == null) {
            return;
        }
        Dictionary<string, object> query = new() {
            ["creationTime"] = vm.CreationTime
        };
        NavigateEventArgs args = new(this, this.Route, PageRoute.QueueDetail, query);
        EventSystem.Publish(this, args);
    }
}