using MusicEco.ViewModels;
using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class QueuePage: ContentView {
    public QueuePage(QueuePageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void CollectionView_DisplayModeChanged(object? sender, CollectionDisplayMode displayMode) {

    }

    private void Button_Clicked(object sender, EventArgs e) {

    }

    private void Button_Clicked_1(object sender, EventArgs e) {

    }
}