using MusicEco.ViewModels.DetailPages;
using MusicEco.Views.Buttons;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.DetailPages;

public partial class PlaycountPage : BaseUserDetailPage, IOptionMenuSupportPage
{
	private readonly PlaycountPageModel ViewModel;
    private readonly OptionMenuExtension optionMenuComponent;
	public PlaycountPage(PlaycountPageModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		MainBindingContext = viewModel;
        optionMenuComponent = new(this);
	}
    protected override async void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
		await ViewModel.LoadData();
    }

    public void OptionMenu_Clicked(object sender, TappedEventArgs e) {
        optionMenuComponent.StartOptionMenu(sender, e);
    }

    public void GetPageEventHandler(object sender, GetBasePageEventArgs e) {
        e.Page = this;
    }
}