using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class HomePage: ContentView {
    public HomePage(HomePageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}