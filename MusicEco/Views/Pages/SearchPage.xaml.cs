using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class SearchPage: ContentView {
    public SearchPage(SearchPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}