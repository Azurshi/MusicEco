using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Settings;

public partial class LanguageSettingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.LanguageSetting;
    public IReadOnlyList<LanguageViewModel> Languages { get; init; }
    public LanguageSettingPageViewModel(ILocalizationService localizationService, IAppSetting appSetting) : base(localizationService, appSetting) {
        this.Languages = [
            new("en", "English"),
            new("vi", "Tiếng Việt")
            ];
    }
    private void RefreshState() {
        foreach(var item in this.Languages) {
            if (item.LanguageCode.Equals(this._localizationService.GetCurrentLanguageCode())) {
                item.Selected = true;
            } else {
                item.Selected = false;
            }
        }
    }
    private bool CanSelectLanguage(LanguageViewModel? vm) {
        if (vm == null) {
            return false;
        }
        return !vm.LanguageCode.Equals(this._localizationService.GetCurrentLanguageCode());
    }
    [RelayCommand(CanExecute = nameof(CanSelectLanguage))]
    private void SelectLanguage(LanguageViewModel? vm) {
        if (vm == null) {
            return;
        }
        this._localizationService.SetLanguage(vm.LanguageCode, this);
        SelectLanguageCommand.NotifyCanExecute();
        RefreshState();
    }
    public override async Task Refresh() {
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        RefreshState();
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
}
