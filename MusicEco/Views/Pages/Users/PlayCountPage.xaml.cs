using MusicEco.ViewModels.Pages.Users;

namespace MusicEco.Views.Pages.Users;

public partial class PlayCountPage: ContentView {
    public PlayCountPage(PlayCountPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}