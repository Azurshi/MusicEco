
using MusicEco.Common.Events;
using MusicEco.Global;
using System.Diagnostics;

namespace MusicEco.Views.Sections;

public partial class UserSection : ContentView
{
    private readonly ViewModels.Sections.UserSection _viewModel;
	public UserSection()
	{
		InitializeComponent();
        _viewModel = (ViewModels.Sections.UserSection)this.BindingContext;
	}
    #region Layout
    private void HideAll() {
        PlaycountButton.IsVisible = false;
        FavouriteButton.IsVisible = false;
        FavouriteArea.IsVisible = false;
        PlaycountArea.IsVisible = false;
        Border1.IsVisible = false;
    }
    private void ShowOverview() {
        PlaycountButton.IsVisible = true;
        FavouriteButton.IsVisible = true;
        Border1.IsVisible = true;
    }
    private async void PlaycountButton_Clicked(object sender, EventArgs e) {
        HideAll();
        PlaycountArea.IsVisible = true;
        await _viewModel.UpdatePlaycountData();
    }
    private async void FavouriteButton_Clicked(object sender, EventArgs e) {
        HideAll();
        FavouriteArea.IsVisible = true;
        await _viewModel.UpdateFavouriteData();
    }
    private void BackButton_Clicked(object sender, EventArgs e) {
        HideAll();
        ShowOverview();
    }
    #endregion
    #region Signal
    private void Favourite_SlotSelected(object sender, StringEventArgs e) {
        Debug.WriteLine("Favourite endpoint " + e.Value);
        List<string> stringSongIds = _viewModel.FavouriteData.Select(o => o.Key).ToList();
        List<int> songIds = [];
        foreach (var stringSongId in stringSongIds) {
            songIds.Add(int.Parse(stringSongId));
        }
        ViewCenter.AddOrUpdateQueue(
            "Favourite",
            int.Parse(e.Value),
            songIds);
    }
    private void Playcount_SlotSelected(object sender, StringEventArgs e) {
        Debug.WriteLine("Playcount endpoint " + e.Value);
        List<string> stringSongIds = _viewModel.PlaycountData.Select(o => o.Key).ToList();
        List<int> songIds = [];
        foreach (var stringSongId in stringSongIds) {
            songIds.Add(int.Parse(stringSongId));
        }
        ViewCenter.AddOrUpdateQueue(
            "Playcount",
            int.Parse(e.Value),
            songIds);
    }
    private async void Favourite_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.FavouriteController.PageDown(e.Value);
    }
    private async void Playcount_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.PlaycountController.PageDown(e.Value);
    }
    #endregion
}