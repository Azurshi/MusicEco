using MusicEco.Core.Services;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;
using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Pages;

public partial class ExplorerTreePageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.ExplorerTree;
    public ObservableCollectionExtend<IUpdateble> Items { get; init; } = new();
#if IOS || MACCATALYST
    public ExplorerTreePageViewModel(ILocalizationService localizationService, IAppSetting appSetting) : base(localizationService, appSetting) {

    }
    [RelayCommand]
    public override async Task Refresh() {
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
    [RelayCommand]
    private async Task SelectFolder(FolderEntryViewModel? vm) {

    }
    [RelayCommand]
    private async Task SelectFile(FileEntryViewModel? vm) {

    }
    private bool CanPrevious() {
        return false;
    }
    [RelayCommand(CanExecute = nameof(CanPrevious))]
    private async Task PreviousFolder() {
        await Refresh();
    }
    private bool CanNext() {
        return false;
    }
    [RelayCommand(CanExecute = nameof(CanNext))]
    private async Task NextFolder() {
        await Refresh();
    }
    private bool CanUp() {
        return false;
    }
    [RelayCommand(CanExecute = nameof(CanUp))]
    private async Task UpFolder() {
        await Refresh();
    }
#endif
}
