using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Pages;

public partial class UserPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.User;
    public SyncCommand<PageRoute> NagivateCommand { get; init; }
    public UserPageViewModel(ILocalizationService localizationService, IAppSetting appSetting) : base(localizationService, appSetting) {
        this.NagivateCommand = new(Navigate);
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
    private void Navigate(PageRoute? route) {
        if (route == null) {
            return;
        }
        var navigateEventArgs = new NavigateEventArgs(this, this.Route, route);
        EventSystem.Publish(this, navigateEventArgs);
    }
}
