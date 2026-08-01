using MusicEco.Core.Services;

namespace MusicEco.ViewModels.Pages;

public partial class AlbumPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Album;
    public AlbumPageViewModel(ILocalizationService localizationService) : base(localizationService) {

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
