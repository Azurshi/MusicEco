using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;

namespace MusicEco.ViewModels.Pages;

public partial class SettingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Setting;
    public SettingPageViewModel(ILocalizationService localizationService, IAppSetting appSetting) : base(localizationService, appSetting) {
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
    [RelayCommand]
    private void SelectSetting(PageRoute? pageRoute) {
        if (pageRoute == null) {
            return;
        }
        var navigateEventArgs = new NavigateEventArgs(this, this.Route, pageRoute);
        EventSystem.Publish(this, navigateEventArgs);
    }
}
