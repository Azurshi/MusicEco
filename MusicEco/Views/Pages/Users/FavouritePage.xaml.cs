using MusicEco.ViewModels.Pages.Users;

namespace MusicEco.Views.Pages.Users;

public partial class FavouritePage: ContentView {
    public FavouritePage(FavouritePageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}