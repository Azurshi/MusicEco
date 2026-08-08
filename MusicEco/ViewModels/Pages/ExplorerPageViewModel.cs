using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class ExplorerPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Explorer;
    private readonly IScanner _scanner;
    public readonly IScanPathService _scanPathService;
    public ObservableCollectionExtend<ScanPathViewModel> Items { get; init; }
    public AsyncCommand AddNewPathCommand { get; init; }
    public AsyncCommand<ScanPathViewModel> DeleteItemCommand { get; init; }
    public SyncCommand<ScanPathViewModel> SelectItemCommand { get; init; }
    public ExplorerPageViewModel(ILocalizationService localizationService, IScanner scanner, IScanPathService scanPathService) : base(localizationService) {
        this._scanner = scanner;
        this._scanPathService = scanPathService;
        this.Items = new();
        this.AddNewPathCommand = new(AddNewPath);
        this.DeleteItemCommand = new(DeleteItem);
        this.SelectItemCommand = new(SelectItem);
        this._scanPathService.ItemChanged += this.ScanPathService_ItemChanged;
    }

    private async void ScanPathService_ItemChanged(object? sender, EventArgs e) {
        await Refresh();
    }

    public override async Task Refresh() {
        var paths = await this._scanPathService.GetPaths();
        List<ScanPathViewModel> items = [];
        foreach(var path in paths) {
            ScanPathViewModel item = new(path);
            items.Add(item);
        }
        items = items.OrderBy(i => i.Path).ToList();
        Items.Update(items);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        await Refresh();
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
    private async Task AddNewPath() {
#if WINDOWS || ANDROID
        var path = await ExplorerPicker.PickFolder();
        if (path != null) {
            await this._scanPathService.AddPath(path);
        }
#endif
    }
    private async Task DeleteItem(ScanPathViewModel? vm) {
        if (vm == null) {
            return;
        }
        await this._scanPathService.RemovePath(vm.Path);
    }
    private void SelectItem(ScanPathViewModel? vm) {
        if (vm == null) {
            return;
        }
        Dictionary<string, object> query = new() {
            ["rootFolder"] = vm.Path
        };
        NavigateEventArgs args = new(this, this.Route, PageRoute.ExplorerTree, query);
        EventSystem.Publish(this, args);
    }
}
