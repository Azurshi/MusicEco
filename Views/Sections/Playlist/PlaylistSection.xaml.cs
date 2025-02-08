using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Views.Widgets;
using System.Diagnostics;

namespace MusicEco.Views.Sections;

public partial class PlaylistSection : ContentView
{
    private readonly ViewModels.Sections.PlaylistSection _viewModel;
	public PlaylistSection()
	{
		InitializeComponent();
        _viewModel = (ViewModels.Sections.PlaylistSection)this.BindingContext;
        _viewModel.UpdateOverviewData().GetAwaiter().GetResult();
	}
    #region Layout
    private async void BackButton_Clicked(object sender, EventArgs e) {
        BackButton.IsVisible = false;
        DetailList.IsVisible = false;
        SearchField.IsVisible = true;
        OverviewList.IsVisible = true;
        await _viewModel.UpdateOverviewData();
    }
    #endregion
    #region Signal
    private async void OverviewList_SlotSelected(object sender, StringEventArgs e) {
        UserData.CurrentSelectedPlaylist = int.Parse(e.Value);
        _viewModel.SelectPlaylist(int.Parse(e.Value));
        await _viewModel.UpdateDetailData();
        BackButton.IsVisible = true;
        DetailList.IsVisible = true;
        SearchField.IsVisible = false;
        OverviewList.IsVisible = false;
    }
    private void DetailList_SlotSelected(object sender, StringEventArgs e) {
        Debug.WriteLine("Playlist endpoint " + e.Value);
        List<string> stringSongIds = _viewModel.DetailData.Select(o => o.Key).ToList();
        List<int> songIds = [];
        foreach (var stringSongId in stringSongIds) {
            songIds.Add(int.Parse(stringSongId));
        }
        ViewCenter.AddOrUpdateQueue(
            $"Playlist {_viewModel.LastSelectedPlaylistName}",
            int.Parse(e.Value),
            songIds);
    }
    private void AddButton_Clicked(object sender, EventArgs e) {
        ViewModels.Sections.PlaylistSection.CreateNewPlaylist();
    }
    private async void SearchField_TextChanged(object sender, TextChangedEventArgs e) {
        await _viewModel.ChangeSearch(e.NewTextValue);
    }
    private async void OverviewList_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.OverviewController.PageDown(e.Value);
    }
    private async void DetailList_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.DetailController.PageDown(e.Value);
    }
    #endregion
    private void OnEntered(object sender, EventArgs e) => ViewUtil.OnEntered(sender, e);
    private void OnExited(object sender, EventArgs e) => ViewUtil.OnExited(sender, e);
}