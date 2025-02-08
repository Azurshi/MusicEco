using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global;
using System.Diagnostics;

namespace MusicEco.Views.Sections;

public partial class SearchSection : ContentView
{
    private readonly ViewModels.Sections.SearchSection _viewModel;
    private bool _loading = false;
    public SearchSection() {
        InitializeComponent();
        _viewModel = (ViewModels.Sections.SearchSection)this.BindingContext;
    }
    #region Signal
    private void ContentList_SlotSelected(object sender, StringEventArgs e) {
        Debug.WriteLine("Search endpoint " + e.Value);
        List<string> stringSongIds = _viewModel.Data.Select(o => o.Key).ToList();
        List<int> songIds = [];
        foreach (var stringSongId in stringSongIds) {
            songIds.Add(int.Parse(stringSongId));
        }
        ViewCenter.AddOrUpdateQueue(
            $"Search {_viewModel.LastSearchedName}",
            int.Parse(e.Value),
            songIds);
    }
    private async void SearchField_TextChanged(object sender, TextChangedEventArgs e) {
        if (e.NewTextValue.Length >= Setting.MinimumSearchLenth) {
            _viewModel.Search(e.NewTextValue);
            if (_loading) return;
            _loading = true;
            await _viewModel.UpdateData();
            _loading = false;
        }
    }
    private async void ContentList_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.DataController.PageDown(e.Value);
    }
    #endregion
}