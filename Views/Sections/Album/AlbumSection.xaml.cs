using MusicEco.Common.Events;

namespace MusicEco.Views.Sections;

public partial class AlbumSection : ContentView
{
    private readonly ViewModels.Sections.AlbumSection _viewModel;
    public AlbumSection() {
        InitializeComponent();
        _viewModel = (ViewModels.Sections.AlbumSection)this.BindingContext;
        _viewModel.UpdateOverviewData().GetAwaiter().GetResult();
    }
    #region Layout
    private void BackButton_Pressed(object sender, EventArgs e) {
        BackButton.IsVisible = false;
        DetailList.IsVisible = false;
        SearchField.IsVisible = true;
        OverviewGrid.IsVisible = true;
    }
    #endregion
    #region Signal
    private async void OverviewGrid_SlotSelected(object sender, StringEventArgs e) {
        _viewModel.SelectAblumn(e.Value);
        await _viewModel.UpdateDetailData();

        BackButton.IsVisible = true;
        DetailList.IsVisible = true;
        SearchField.IsVisible = false;
        OverviewGrid.IsVisible = false;
    }
    private void DetaiList_SlotSelected(object sender, StringEventArgs e) {
        List<string> stringSongIds = _viewModel.DetailData.Select(o => o.Key).ToList();
        List<int> songIds = [];
        foreach (var stringSongId in stringSongIds) {
            songIds.Add(int.Parse(stringSongId));
        }
        ViewCenter.AddOrUpdateQueue(
            $"Album {_viewModel.LastSelectedAlbum}",
            int.Parse(e.Value),
            songIds);
    }
    private async void SearchField_TextChanged(object sender, TextChangedEventArgs e) {
        await _viewModel.ChangeSearch(e.NewTextValue);
    }
    private async void OverviewGrid_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.OverviewController.PageDown(e.Value * _viewModel.ColumnsCount);
    }
    private async void DetailList_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.DetailController.PageDown(e.Value);
    }
    #endregion
}