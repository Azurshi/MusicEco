using MusicEco.ViewModels.DetailPages;
using MusicEco.Views.Buttons;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.DetailPages;

public partial class AlbumDetailPage : BaseDetailPage, IQueryAttributable, IOptionMenuSupportPage {
    private readonly AlbumDetailPageModel ViewModel;
    private readonly OptionMenuExtension optionMenuComponent;
    public AlbumDetailPage(AlbumDetailPageModel viewModel) {
        InitializeComponent();
        ViewModel = viewModel;
        MainBindingContext = viewModel;
        optionMenuComponent = new(this);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        ViewModel.ApplyQueryAttributes(query);
    }

    #region Interfacce
    public void OptionMenu_Clicked(object sender, TappedEventArgs e) {
        optionMenuComponent.StartOptionMenu(sender, e);
    }
    public void GetPageEventHandler(object sender, GetBasePageEventArgs e) {
        e.Page = this;
    }
    #endregion

}