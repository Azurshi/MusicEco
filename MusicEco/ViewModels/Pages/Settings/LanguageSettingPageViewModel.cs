using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Settings;

public partial class LanguageSettingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.LanguageSetting;
    private readonly IAppSetting _settings;
    public IReadOnlyList<LanguageViewModel> Languages { get; init; }
    public SyncCommandExtend<LanguageViewModel> SelectLanguageCommand { get; init; }
    private string SelectedLanguageCode {
        get => this._settings.Get("en");
        set => this._settings.Set(value);
    }
    public LanguageSettingPageViewModel(ILocalizationService localizationService, IAppSetting appSetting) : base(localizationService) {
        this._settings = appSetting;
        this.Languages = [
            new("en", "English"),
            new("vi", "Tiếng Việt")
            ];
        this.SelectLanguageCommand = new(SelectLanguage, CanSelectLanguage);
    }
    private void RefreshState() {
        foreach(var item in this.Languages) {
            if (item.LanguageCode.Equals(this.SelectedLanguageCode)) {
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
        return !vm.LanguageCode.Equals(this.SelectedLanguageCode);
    }
    private void SelectLanguage(LanguageViewModel? vm) {
        if (vm == null) {
            return;
        }
        this._localizationService.SetLanguage(vm.LanguageCode, this);
        this.SelectedLanguageCode = this._localizationService.GetCurrentLanguageCode();
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
