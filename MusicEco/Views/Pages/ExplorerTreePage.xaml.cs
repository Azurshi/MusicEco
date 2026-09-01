using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class ExplorerTreePage: ContentView {
    public ExplorerTreePage(ExplorerTreePageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}