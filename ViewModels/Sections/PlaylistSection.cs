using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Common.Value;
using MusicEco.Global;
using MusicEco.Models;
using MusicEco.Models.Base;
using MusicEco.ViewModels.Base;
using MusicEco.ViewModels.Slots;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels.Sections;
public class PlaylistSection : PropertyObject {
    public PlaylistSection() {
        OverviewController = new([], Setting.ListFrameAmount);
        DetailController = new([], Setting.ListFrameAmount);
        BaseModel.OnModelDataChangedConnect<PlaylistModel>(OnModelDataChanged);
        EventSystem.Connect(Signal.System_Data_Loaded, OnDataLoaded);
    }
    private async void OnModelDataChanged(object? sender, IntChangeEventArgs e) {
        await UpdateOverviewData();
        await UpdateDetailData();
    }
    private async void OnDataLoaded(object? sender, EventArgs e) {
        await UpdateOverviewData();
        await UpdateDetailData();
    }


    public int ColumnsCount => Setting.GridCoumnsCount;
    public readonly ObservableCollectionController<PlaylistSlot> OverviewController;
    public ObservableCollection<BaseSlot> OverviewData => OverviewController.Target;
    public readonly ObservableCollectionController<SongSlot> DetailController;
    public ObservableCollection<BaseSlot> DetailData => DetailController.Target;
    private int _lastSelectedPlaylist = -1;
    public int LastSelectedPlaylist => _lastSelectedPlaylist;
    public string LastSelectedPlaylistName => PlaylistModel.Get(_lastSelectedPlaylist)?.Name ?? "Unknow";
    private string _searchCriteria = string.Empty;

    public async Task UpdateOverviewData() {
        List<int> playlistIds;
        if (_searchCriteria.Length > Setting.MinimumSearchLenth) {
            playlistIds = BaseModel.GetAll<PlaylistModel>().Where(e => e.Type == Data.Playlist_PlaylistType && e.Name.Contains(_searchCriteria, StringComparison.OrdinalIgnoreCase)).OrderBy(e => e.TimeStamp).Select(o => o.Id).ToList();
        }
        else {
            playlistIds = BaseModel.GetAll<PlaylistModel>().Where(e => e.Type == Data.Playlist_PlaylistType).OrderBy(e => e.TimeStamp).Select(o => o.Id).ToList();
        }
        List<string> stringPlaylistIds = [];
        foreach (var playlist in playlistIds) {
            stringPlaylistIds.Add(playlist.ToString());
        }
        await OverviewController.UpdateKeysAsync(stringPlaylistIds);
    }
    public void SelectPlaylist(int playlistId) {
        _lastSelectedPlaylist = playlistId;
    }
    public async Task UpdateDetailData() {
        List<int> songIds = PlaylistModel.Get(_lastSelectedPlaylist)?.SongIds ?? [];
        List<string> stringSongIds = songIds.Select(e => e.ToString()).ToList();
        await DetailController.UpdateKeysAsync(stringSongIds);
    }
    #region DataModify
    public static void CreateNewPlaylist() {
        PlaylistModel model = new();
        List<string> existsNames = BaseModel.GetAll<PlaylistModel>().Where(e => e.Type == Data.Playlist_PlaylistType).Select(e => e.Name).ToList();
        model.AssignId();
        int it = 0;
        string name = Data.Default_NewPlaylistName;
        while (existsNames.Contains(name)) {
            it ++;
            name = $"{Data.Default_NewPlaylistName} {it}";
        }
        model.Name = name;
        model.Type = Data.Playlist_PlaylistType;
        model.SongIds = [];
        model.Save();
    }
    public async Task ChangeSearch(string name) {
        if (!name.Equals(_searchCriteria)) {
            _searchCriteria = name;
            await UpdateOverviewData();
        }
    }
    #endregion
}