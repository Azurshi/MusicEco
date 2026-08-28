using MusicEco.Core.Services;
using MusicEco.SourceGeneration;

namespace MusicEco.ViewModels.Pages.Settings;

public partial class BackupSetttingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.BackupSetting;
    private readonly IScanner _scanner;
    public BackupSetttingPageViewModel(ILocalizationService localizationService, IAppSetting setting, IScanner scanner) : base(localizationService, setting) {
        this._scanner = scanner;
    }

    private void Scanner_RunningChanged(object? sender, bool e) {
        this.DeleteAllDataCommand.NotifyCanExecute();
    }

    public override async Task Refresh() {
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        this._scanner.RunningChanged += this.Scanner_RunningChanged;
        this.DeleteAllDataCommand.NotifyCanExecute();
        FireNavigated(e);
    }
    public override async Task OnNavigatedFrom(NavigateEventArgs e) {
        await base.OnNavigatedFrom(e);
        this._scanner.RunningChanged += this.Scanner_RunningChanged;
    }
    private bool IsNotScanning() {
        return !this._scanner.Running;
    }
    [RelayCommand(CanExecute = nameof(IsNotScanning))]
    private async Task DeleteAllData() {
        await this._setting.DeleteAllData();
        Application.Current?.Quit();
    }
}
