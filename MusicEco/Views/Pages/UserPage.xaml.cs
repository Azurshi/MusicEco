using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class UserPage: ContentView {
    public UserPage(UserPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}