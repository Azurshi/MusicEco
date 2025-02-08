using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Models;
using MusicEco.Models.Base;
using MusicEco.ViewModels.Base;
using MusicEco.ViewModels.Slots;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.Sections;
public class UserSection : PropertyObject {
    public UserSection() {
        FavouriteController = new([], Setting.ListFrameAmount);
        PlaycountController = new([], Setting.ListFrameAmount);
        EventSystem.Connect(Signal.System_Data_Loaded, OnDataLoaded);
        EventSystem.Connect<IntEventArgs>(Signal.Song_Favourite_Changed, OnSongFavourite_Changed);
        EventSystem.Connect<IntEventArgs>(Signal.Song_Playcount_Changed, OnSongPlaycount_Changed);
    }
    private async void OnDataLoaded(object? sender, EventArgs e) {
        await UpdateFavouriteData();
        await UpdatePlaycountData();
    }
    private async void OnSongFavourite_Changed(object? sender, IntEventArgs e) {
        await UpdateFavouriteData();
    }
    private async void OnSongPlaycount_Changed(object? sender, IntEventArgs e) {
        await UpdatePlaycountData();
    }
    public readonly ObservableCollectionController<AdditionSongSlot> FavouriteController;
    public ObservableCollection<BaseSlot> FavouriteData => FavouriteController.Target;
    public readonly ObservableCollectionController<AdditionSongSlot> PlaycountController;
    public ObservableCollection<BaseSlot> PlaycountData => PlaycountController.Target;
    #region DataModify
    public async Task UpdateFavouriteData() {
        List<int> songIds = BaseModel.GetAll<SongModel>().Where(e => e.Favourite).Select(o => o.Id).ToList();
        List<string> stringSongIds = [];
        foreach (var songId in songIds) {
            stringSongIds.Add(songId.ToString());
        }
        await FavouriteController.UpdateKeysAsync(stringSongIds);
    }
    public async Task UpdatePlaycountData() {
        List<int> songIds = BaseModel.GetAll<SongModel>().Where(e => e.PlayCount > 0).OrderByDescending(e => e.PlayCount).Select(o => o.Id).ToList();
        List<string> stringSongIds = [];
        foreach (var songId in songIds) {
            stringSongIds.Add(songId.ToString());
        }
        await PlaycountController.UpdateKeysAsync(stringSongIds);
    }
    #endregion
}