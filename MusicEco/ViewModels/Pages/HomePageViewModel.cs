using MusicEco.Core.Services;

namespace MusicEco.ViewModels.Pages;

public partial class HomePageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Home;
    public HomePageViewModel(ILocalizationService localizationService) : base(localizationService) {

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
