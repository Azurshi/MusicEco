using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Pages;

public partial class SettingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Setting;
    public SyncCommand<PageRoute> SelectSettingCommand { get; init; }
    public SettingPageViewModel(ILocalizationService localizationService) : base(localizationService) {
        this.SelectSettingCommand = new(SelectSetting);
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
    private void SelectSetting(PageRoute? pageRoute) {
        if (pageRoute == null) {
            return;
        }
        var navigateEventArgs = new NavigateEventArgs(this, this.Route, pageRoute);
        EventSystem.Publish(this, navigateEventArgs);
    }
}
