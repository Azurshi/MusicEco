using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data;
using MusicEco.Services;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Overlays;

namespace MusicEco.ViewModels.Pages;

public partial class ExplorerPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Explorer;
    private readonly IScanner _scanner;
    public readonly IScanPathService _scanPathService;
    private readonly IOverlayService _overlayService;
    public ScanProgressInfo ProgressInfo { get; init; }
    public ObservableCollectionExtend<ScanPathViewModel> Items { get; init; }
    public bool IsLocked => this._scanner.Running;
    public bool IsUnLocked => !this._scanner.Running;
    public ExplorerPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IScanner scanner, IScanPathService scanPathService, IOverlayService overlayService) : base(localizationService, appSetting) {
        this._scanner = scanner;
        this._scanPathService = scanPathService;
        this._overlayService = overlayService;
        this.Items = new();
        this.ProgressInfo = new();
        this._scanPathService.ItemChanged += this.ScanPathService_ItemChanged;
        this._scanner.RunningChanged += this.Scanner_RunningChanged;
    }
    private void ChangeState(ScanPathViewModel vm) {
        vm.IsLocked = this._scanner.Running;
    }
    private void RefreshState() {
        Items.RefreshState(ChangeState);
    }
    private void Scanner_RunningChanged(object? sender, bool e) {
        ScanCommand.NotifyCanExecute();
        AddNewPathCommand.NotifyCanExecute();
        OnPropertyChanged(nameof(IsLocked));
        OnPropertyChanged(nameof(IsUnLocked));
        RefreshState();
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
        Items.Update(items, ChangeState);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        await Refresh();
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
    [RelayCommand(CanExecute = nameof(IsNotScanning))]
    private async Task AddNewPath() {
#if WINDOWS || ANDROID
        var path = await ExplorerPicker.PickFolder();
        if (path != null) {
            await this._scanPathService.AddPath(path);
        }
#endif
    }
    private bool IsNotScanning() {
        return !this._scanner.Running;
    }
    [RelayCommand]
    private async Task DeleteItem(ScanPathViewModel? vm) {
        if (vm == null) {
            return;
        }
        await this._scanPathService.RemovePath(vm.Path);
    }
    [RelayCommand]
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
    [RelayCommand(CanExecute = nameof(IsNotScanning))]
    private async Task Scan() {
        var info = this.ProgressInfo;
        info.Reset();
        TimeSpan delay = TimeSpan.FromMilliseconds(100);
        ScanProgress process = new(
            (p) => {
                info.ScanFileCurrent = p.Current;
                info.ScanFileTotal = p.Total;
            },
            (p) => {
                info.ProcessFileCurrent = p.Current;
                info.ProcessFileTotal = p.Total;
            },
            (p) => {
                info.SaveDataCurrent = p.Current;
                info.SaveDataTotal = p.Total;
            });
#if WINDOWS
        int nScanWorkers = 2;
        int nProcessWorkers = 4;
#else
        int nScanWorkers = 1;
        int nProcessWorkers = 1;
#endif
        bool verbose = true;
        var result = await this._scanner.ScanAndUpdate(process, [".mp3", ".wav", ".flac"], nScanWorkers, nProcessWorkers, delay, this, verbose);
        if (result.Success && result is DetailScanResult scanResult) {
#if ANDROID || WINDOWS
            var extra = (ValueTuple<TimeSpan, TimeSpan, TimeSpan>)scanResult.Extra!;
            string message = $"""
                Scan file    : {scanResult.ScanFileTime.TotalSeconds:F2}
                Process file : {scanResult.ProcessFileTime.TotalSeconds:F2}
                Save data    : {scanResult.SaveDataTime.TotalSeconds:F2}
                Read     : {extra.Item1.TotalSeconds:F2}
                Hash     : {extra.Item2.TotalSeconds:F2}
                Finalize : {extra.Item3.TotalSeconds:F2}
                """;
            var view = AppLifeCycle.Provider.GetRequiredService<DevInfoOverlay>();
            this._overlayService.ShowDynamic(new(0.9f, 0.9f), view);
            view.Initialize(message);
#endif
        }
    }
}
