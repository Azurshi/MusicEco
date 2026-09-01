using MusicEco.ViewModels.Pages.Users;

namespace MusicEco.Views.Pages.Users;

public partial class NotPlayPage: ContentView {
    public NotPlayPage(NotPlayPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;    
    }
}