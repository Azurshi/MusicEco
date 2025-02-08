using MusicEco.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicEco.ViewModels.Base;
using MusicEco.ViewModels.Slots;
using MusicEco.Models;
using MusicEco.Models.Base;
using MusicEco.Global;
using MusicEco.Common.Events;

namespace MusicEco.ViewModels.Sections;
public class AlbumSection : PropertyObject {
    public AlbumSection() {
        OverviewController = new([], Setting.ListFrameAmount);
        DetailController = new([], Setting.ListFrameAmount);
        EventSystem.Connect(Signal.System_Data_Loaded, OnDataLoaded);
    }
    private async void OnDataLoaded(object? sender, EventArgs e) {
        await UpdateOverviewData();
        await UpdateDetailData();
    }
    public int ColumnsCount => Setting.GridCoumnsCount;
    public readonly ObservableCollectionController<AlbumOverviewSlot> OverviewController;
    public ObservableCollection<BaseSlot> OverviewData => OverviewController.Target;
    public readonly ObservableCollectionController<SongSlot> DetailController;
    public ObservableCollection<BaseSlot> DetailData => DetailController.Target;
    public string LastSelectedAlbum = Common.Value.Null.String;
    private string _searchCriteria = string.Empty;
    #region DataModify
    public async Task UpdateOverviewData() {
        List<string> albums;
        if (_searchCriteria.Length > Setting.MinimumSearchLenth) {
            albums = BaseModel.GetAll<SongModel>().Where(e => e.Album.Contains(_searchCriteria, StringComparison.OrdinalIgnoreCase)).Select(o => o.Album).Distinct().ToList();
        } else {
            albums = BaseModel.GetAll<SongModel>().Select(o => o.Album).Distinct().ToList();
        }
        albums.Sort();
        await OverviewController.UpdateKeysAsync(albums);
    }
    public void SelectAblumn(string albumName) {
        LastSelectedAlbum = albumName;
    }
    public async Task ChangeSearch(string name) {
        if (!name.Equals(_searchCriteria)) {
            _searchCriteria = name;
            await UpdateOverviewData();
        }
    }
    public async Task UpdateDetailData() {
        List<int> songIds = BaseModel.GetAll<SongModel>().Where(e => e.Album == LastSelectedAlbum).Select(o => o.Id).ToList();
        List<string> stringSongIds = songIds.Select(e => e.ToString()).ToList();
        await DetailController.UpdateKeysAsync(stringSongIds);
    }
    #endregion
}
