using MusicEco.Core.Services;

namespace MusicEco.ViewModels.Pages;

public partial class SettingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Setting;
    public SettingPageViewModel(ILocalizationService localizationService) : base(localizationService) {

    }
    public override async Task Refresh() {
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
}
