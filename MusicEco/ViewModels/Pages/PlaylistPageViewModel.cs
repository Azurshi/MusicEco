using MusicEco.Core.Services;

namespace MusicEco.ViewModels.Pages;

public partial class PlaylistPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Playlist;
    public PlaylistPageViewModel(ILocalizationService localizationService) : base(localizationService) {

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
