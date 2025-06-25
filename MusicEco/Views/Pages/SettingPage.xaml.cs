using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class SettingPage : BasePage
{
	private readonly SettingPageModel ViewModel;
	public SettingPage(SettingPageModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		MainBindingContext = viewModel;
	}
    private void Setting_Clicked(object sender, EventArgs e) {
        NavigationList.IsVisible = false;
        ControlTab.IsVisible = true;
        ApplicationSettingList.IsVisible = true;
    }
    private void Back() {
        NavigationList.IsVisible = true;
        ControlTab.IsVisible = false;
        ApplicationSettingList.IsVisible = false;
    }
    private void BackButton_Clicked(object sender, EventArgs e) {
        Back();
    }

    private void Confirm_Clicked(object sender, TappedEventArgs e) {
        Back();
    }

    private void Cancel_Clicked(object sender, TappedEventArgs e) {
        Back();
    }
}