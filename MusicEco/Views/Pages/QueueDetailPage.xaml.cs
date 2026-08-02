using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class QueueDetailPage: ContentView {
    public QueueDetailPage(QueueDetailPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}