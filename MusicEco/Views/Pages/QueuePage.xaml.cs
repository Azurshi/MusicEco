using MusicEco.ViewModels;
using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class QueuePage: ContentView {
    public QueuePage(QueuePageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}