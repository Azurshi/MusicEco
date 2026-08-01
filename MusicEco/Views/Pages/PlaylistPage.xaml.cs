using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class PlaylistPage: ContentView {
    public PlaylistPage(PlaylistPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}