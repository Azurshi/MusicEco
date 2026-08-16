using MusicEco.ViewModels.Pages.Users;

namespace MusicEco.Views.Pages.Users;

public partial class AllSongPage: ContentView {
    public AllSongPage(AllSongPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}