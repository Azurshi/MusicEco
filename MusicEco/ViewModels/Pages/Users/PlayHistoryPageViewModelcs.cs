using MusicEco.Core.Services;

namespace MusicEco.ViewModels.Pages.Users;

public partial class PlayHistoryPageViewModelcs: BasePageViewModel {
    public override PageRoute Route => PageRoute.PlayHistory;
    public PlayHistoryPageViewModelcs(ILocalizationService localizationService) : base(localizationService) {

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
