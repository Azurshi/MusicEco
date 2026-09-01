using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class ExplorerPage: ContentView {
    public ExplorerPage(ExplorerPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}