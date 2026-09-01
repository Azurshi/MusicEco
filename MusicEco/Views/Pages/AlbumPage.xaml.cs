using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class AlbumPage: ContentView {
    public AlbumPage(AlbumPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}