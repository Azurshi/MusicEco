using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Pages;

public partial class HomePageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Home;
    private readonly IQueueService _queueService;
    public Hash256 FileHash { get; private set; }
    public string DisplayTitle { get; private set; }
    public HomePageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IQueueService queueService) : base(localizationService, appSetting) {
        this._queueService = queueService;
        this._queueService.CurrentChanged += this.QueueService_CurrentChanged;
        this.FileHash = new();
        this.DisplayTitle = string.Empty;
    }

    private async void QueueService_CurrentChanged(object? sender, EventArgs e) {
        await Refresh();
    }

    public override async Task Refresh() {
        var currentQueue = await this._queueService.GetCurrent();
        if (currentQueue != null && currentQueue.Current != null) {
            AudioEntry current = currentQueue.Current;
            this.FileHash = current.Hash;
            this.DisplayTitle = current.Title;
        }
        else {
            this.FileHash = new Hash256();
            this.DisplayTitle = string.Empty;
        }
        OnPropertyChanged(nameof(DisplayTitle));
        OnPropertyChanged(nameof(FileHash));
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        await Refresh();
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
}
