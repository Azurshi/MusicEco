using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class UserPage : BasePage, IServiceAccess
{
	private readonly UserPageModel ViewModel;
	public UserPage(UserPageModel viewModel)
	{
		InitializeComponent();

        ViewModel = viewModel;
		this.MainBindingContext = viewModel;
	}
}