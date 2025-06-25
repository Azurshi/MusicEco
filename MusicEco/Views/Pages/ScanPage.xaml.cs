using MusicEco.ViewModels.DetailPages;

namespace MusicEco.Views.Pages;

public partial class ScanPage : BasePage
{
	public readonly ScanPageModel ViewModel;
	public ScanPage(ScanPageModel viewModel)
	{
		InitializeComponent();
		MainBindingContext = viewModel;
		ViewModel = viewModel;
	}
    protected override async void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
        await ViewModel.LoadData();
    }
}