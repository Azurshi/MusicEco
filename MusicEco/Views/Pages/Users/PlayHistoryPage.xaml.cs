using MusicEco.ViewModels.Pages.Users;

namespace MusicEco.Views.Pages.Users;

public partial class PlayHistoryPage: ContentView {
    public PlayHistoryPage(PlayHistoryPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}