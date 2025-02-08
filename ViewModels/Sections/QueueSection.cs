using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Common.Value;
using MusicEco.Global;
using MusicEco.Models;
using MusicEco.Models.Base;
using MusicEco.ViewModels.Base;
using MusicEco.ViewModels.Slots;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.Sections;
public class QueueSection : PropertyObject {
    public QueueSection() {
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

    public async Task FreeResource() {
        await OverviewController.UpdateKeysAsync([]);
    }
    public int ColumnsCount => Setting.GridCoumnsCount;
    public readonly ObservableCollectionController<PlaylistSlot> OverviewController;
    public ObservableCollection<BaseSlot> OverviewData => OverviewController.Target;
    public readonly ObservableCollectionController<SongSlot> DetailController;
    public ObservableCollection<BaseSlot> DetailData => DetailController.Target;
    private int _lastSelectedQueue = -1;
    public int LastSelectedQueue => _lastSelectedQueue;
    private string _searchCriteria = string.Empty;
    #region DataModify
    public async Task UpdateOverviewData() {
        List<int> queueIds;
        if (_searchCriteria.Length > Setting.MinimumSearchLenth) {
            queueIds = BaseModel.GetAll<PlaylistModel>().Where(e => e.Type == Data.Playlist_QueueType && e.Name.Contains(_searchCriteria, StringComparison.OrdinalIgnoreCase)).OrderBy(e => e.TimeStamp).Select(o => o.Id).ToList();
        } else {
            queueIds = BaseModel.GetAll<PlaylistModel>().Where(e => e.Type == Data.Playlist_QueueType).OrderBy(e => e.TimeStamp).Select(o => o.Id).ToList();
        }
        List<string> stringQueueIds = [];
        foreach (var playlist in queueIds) {
            stringQueueIds.Add(playlist.ToString());
        }
        await OverviewController.UpdateKeysAsync(stringQueueIds);
    }
    public void SelectQueue(int queueName) {
        _lastSelectedQueue = queueName;
    }
    public async Task UpdateDetailData() {
        List<int> songIds = PlaylistModel.Get(_lastSelectedQueue)?.SongIds ?? [];
        List<string> stringSongIds = songIds.Select(e => e.ToString()).ToList();
        await DetailController.UpdateKeysAsync(stringSongIds);
    }
    public async Task ChangeSearch(string name) {
        if (!name.Equals(_searchCriteria)) {
            _searchCriteria = name;
            await UpdateOverviewData();
        }
    }
    #endregion
}