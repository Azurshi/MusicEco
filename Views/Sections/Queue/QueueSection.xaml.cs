using MusicEco.Common;
using MusicEco.Common.Events;
using System.Diagnostics;

namespace MusicEco.Views.Sections;

public partial class QueueSection : ContentView
{
    public static void OnEntered(object sender, EventArgs e) => ViewUtil.OnEntered(sender, e);
    public static void OnExited(object sender, EventArgs e) => ViewUtil.OnExited(sender, e);
    private readonly ViewModels.Sections.QueueSection _viewModel;
	public QueueSection()
	{
		InitializeComponent();
        _viewModel = (ViewModels.Sections.QueueSection)this.BindingContext;
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
    #region Slot
    private async void OverviewList_SlotSelected(object sender, StringEventArgs e) {
        UserData.CurrentSelectedQueue = int.Parse(e.Value);
        _viewModel.SelectQueue(int.Parse(e.Value));
        await _viewModel.UpdateDetailData();
        BackButton.IsVisible = true;
        DetailList.IsVisible = true;
        SearchField.IsVisible = false;
        OverviewList.IsVisible = false;
    }
    private void DetailList_SlotSelected(object sender, StringEventArgs e) {
        Debug.WriteLine("Queue endpoint " + e.Value);
        List<string> stringSongIds = _viewModel.DetailData.Select(o => o.Key).ToList();
        List<int> songIds = [];
        foreach (var stringSongId in stringSongIds) {
            songIds.Add(int.Parse(stringSongId));
        }
        ViewCenter.PlayAudio(_viewModel.LastSelectedQueue, int.Parse(e.Value));
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
}