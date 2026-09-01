using MusicEco.ViewModels.Pages.Users;

namespace MusicEco.Views.Pages.Users;

public partial class PlaylistPage: ContentView {
    public PlaylistPage(PlaylistPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}