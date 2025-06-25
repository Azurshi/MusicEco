using MusicEco.ViewModels.Pages;
using System.Diagnostics;
namespace MusicEco.Views.Pages;

public partial class AlbumPage : BasePage
{
    private readonly AlbumPageModel ViewModel;
    public AlbumPage(AlbumPageModel viewModel) {
        InitializeComponent();
        ViewModel = viewModel;
        this.MainBindingContext = viewModel;
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
        await ViewModel.LoadData();
    }
}