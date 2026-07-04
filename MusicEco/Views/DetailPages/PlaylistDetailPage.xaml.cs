using MusicEco.ViewModels.Components;
using MusicEco.ViewModels.DetailPages;
using MusicEco.Views.Buttons;
using MusicEco.Views.PageExtensions;
using System.ComponentModel;
using System.Diagnostics;

namespace MusicEco.Views.DetailPages;

public partial class PlaylistDetailPage : BaseDetailPage, IQueryAttributable, IOptionMenuSupportPage, IDragSupportPage
{
    private readonly PlaylistDetailPageModel ViewModel;
    private readonly OptionMenuExtension optionMenuComponent;
    private readonly DragExtension dragExtension;
    public PlaylistDetailPage(PlaylistDetailPageModel viewModel) {
        InitializeComponent();
        ViewModel = viewModel;
        MainBindingContext = viewModel;
        optionMenuComponent = new(this);
        dragExtension = new();
        ViewModel.PropertyChanged += OnVMPropertyChanged;
    }
    /// <summary>
    /// Lots of patches since use shared Component
    /// </summary>
    private void OnVMPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.ViewModel.BackRoute)) {
            this.BackRoute = ViewModel.BackRoute;
        }
    }
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        ViewModel.ApplyQueryAttributes(query);
        //var navModel = IServiceAccess.Service.GetRequiredService<NavigationBarModel>();
        //if (BackRoute == "queue") {
        //    navModel.DisplayAsQueue();
        //}
        //else {
        //    navModel.DisplayAsPlaylist();
        //}
        //Debug.WriteLine(BackRoute);
        // AHHHHHHHHHHHHHHHHHHH
    }
    #region Interface
    public void OptionMenu_Clicked(object sender, TappedEventArgs e) {
        optionMenuComponent.StartOptionMenu(sender, e);
    }
    public void DropGestureRecognizer_Drop(object sender, DropEventArgs e) {
        dragExtension.Drop(sender, e);
    }
    public void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e) {
        dragExtension.DragStarting(sender, e);
    }

    public void GetPageEventHandler(object sender, GetBasePageEventArgs e) {
        e.Page = this;
    }
    #endregion

}