using MusicEco.ViewModels.Pages;
using MusicEco.Views.Buttons;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.Pages;

public partial class PlaylistPage : BasePage, IOptionMenuSupportPage
{
    private readonly PlaylistPageModel ViewModel;
    private readonly OptionMenuExtension optionMenuComponent;
    public PlaylistPage(PlaylistPageModel viewModel) {
        InitializeComponent();
        ViewModel = viewModel;
        MainBindingContext = ViewModel;
        optionMenuComponent = new(this);
    }

    public void GetPageEventHandler(object sender, GetBasePageEventArgs e) {
        e.Page = this;
    }

    public void OptionMenu_Clicked(object sender, TappedEventArgs e) {
        optionMenuComponent.StartOptionMenu(sender, e);
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
        await ViewModel.LoadData();
    }
}