using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class AlbumDetailPage: ContentView {
    public AlbumDetailPage(AlbumDetailPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}