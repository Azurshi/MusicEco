using MusicEco.ViewModels.Pages;
using MusicEco.Views.Buttons;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.Pages;

public partial class ExplorerPage : BasePage, IQueryAttributable, IOptionMenuSupportPage
{
    private readonly ExplorerPageModel ViewModel;
    private readonly OptionMenuExtension optionMenuComponent;
    public ExplorerPage(ExplorerPageModel viewModel) {
        InitializeComponent();
        ViewModel = viewModel;
        MainBindingContext = viewModel;
        optionMenuComponent = new(this);
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        ViewModel.ApplyQueryAttributes(query);
    }

    public void GetPageEventHandler(object sender, GetBasePageEventArgs e) {
        e.Page = this;
    }

    public void OptionMenu_Clicked(object sender, TappedEventArgs e) {
        optionMenuComponent.StartOptionMenu(sender, e);
    }
}