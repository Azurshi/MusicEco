using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;
public sealed partial class QueuePageQuery: ObservableObject {
    [ObservableProperty]
    public partial string Name { get; set; }
    public QueuePageQuery() {
        this.Name = string.Empty;
    }
}

public partial class QueuePageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Queue;
    private readonly IQueueService _queueService;
    public QueuePageQuery Query { get; init; }
    private readonly DelayedDispatcherEx _queryDispatcher;
    public ManagedCollection<QueueItemViewModel> Items { get; init; }
    [AppSettingProperty(CollectionDisplayMode.SimpleList)]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public QueuePageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IQueueService queueService) : base(localizationService, appSetting) {
        this.Query = new();
        this._queryDispatcher = new(Config.UserInputDelay);
        this._queueService = queueService;
        this.Items = new(this.Filter);
        this.Query.PropertyChanged += this.Query_PropertyChanged;
    }
    private IReadOnlyList<QueueItemViewModel> Filter(IReadOnlyList<QueueItemViewModel> items) {
        string nameQuery = this.Query.Name.Trim();
        if (nameQuery.Length >= Config.MinNameLength) {
            List<QueueItemViewModel> result = [];
            foreach (var item in items) {
                if (item.Name.Contains(nameQuery, StringComparison.InvariantCultureIgnoreCase)) {
                    result.Add(item);
                }
            }
            return result;
        }
        else {
            return items;
        }
    }
    private async void Query_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
        var currentQueue = await _queueService.GetCurrent();
        await this._queryDispatcher.Dispatch(() => {
            this.Items.Refresh((item) => {
                bool isCurrent = false;
                if (currentQueue != null && currentQueue.CreationTime == item.CreationTime) {
                    isCurrent = true;
                }
                item.Selected = isCurrent;
            });
        });
    }

    public override async Task Refresh() {
        var queues = await _queueService.GetAll();
        var currentQueue = await _queueService.GetCurrent();
        List<QueueItemViewModel> items = [];
        foreach(var q in queues) {
            QueueItemViewModel item = new(q.CreationTime, q.ModifiedTime, q.LastPlayTime, q.Name);
            items.Add(item);
        }
        items = items.OrderBy(q => q.LastPlayTime).ToList();
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
    [RelayCommand]
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
    [RelayCommand]
    private async Task RemoveItem(QueueItemViewModel? vm) {
        if (vm == null) {
            return;
        }
        var queue = await this._queueService.Get(vm.CreationTime);
        if (queue != null) {
            await this._queueService.Delete(queue, this);
        }
    }
}