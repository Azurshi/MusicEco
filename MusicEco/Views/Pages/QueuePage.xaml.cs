using MusicEco.ViewModels.Pages;
using MusicEco.Views.Buttons;
using MusicEco.Views.Components;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.Pages;

public partial class QueuePage : BasePage, IDragSupportPage, IOptionMenuSupportPage
{
    private readonly QueuePageModel ViewModel;
    private readonly OptionMenuExtension OptionComponent;
    private readonly DragExtension DragComponent;
    public QueuePage(QueuePageModel viewModel)
	{
		InitializeComponent();
        ViewModel = viewModel;
        MainBindingContext = viewModel;
        OptionComponent = new(this);
        DragComponent = new();
	}
    protected override async void OnNavigatedTo(NavigatedToEventArgs args) {
        base.OnNavigatedTo(args);
        await ViewModel.LoadData();
    }
    #region Interface
    Overlay PageExtensions.IBasePage.PageOverlay => PageOverlay;
    public void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e) {
        DragComponent.DragStarting(sender, e);
    }
    public void DropGestureRecognizer_Drop(object sender, DropEventArgs e) {
        DragComponent.Drop(sender, e);
    }
    public void OptionMenu_Clicked(object sender, TappedEventArgs e) {
        OptionComponent.StartOptionMenu(sender, e);
    }

    public void GetPageEventHandler(object sender, GetBasePageEventArgs e) {
        e.Page = this;
    }
    #endregion
}