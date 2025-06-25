using MusicEco.ViewModels.DetailPages;
using MusicEco.Views.Buttons;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.DetailPages;

public partial class FavouritePage : BaseUserDetailPage, IOptionMenuSupportPage
{
	private readonly FavouritePageModel ViewModel;
    private readonly OptionMenuExtension optionMenuComponent;
	public FavouritePage(FavouritePageModel viewModel)
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