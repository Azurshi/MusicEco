using MusicEco.Common;
using MusicEco.Models;
using MusicEco.Models.Base;
using MusicEco.ViewModels.Base;
using MusicEco.ViewModels.Slots;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.Misc;
public class PlaylistList : PropertyObject {
    public PlaylistList() {
        DataController = new([], Setting.ListFrameAmount);

    }
    public async Task FreeResource() {
        await DataController.UpdateKeysAsync([]);
    }
    public readonly ObservableCollectionController<PlaylistSlot> DataController;
    public ObservableCollection<BaseSlot> Data => DataController.Target;
    private string _queryType = Common.Value.Data.Playlist_PlaylistType;
    #region DataModify
    public async Task UpdateData() {
        List<int> playlistIds = BaseModel.GetAll<PlaylistModel>().Where(e => e.Type == _queryType).OrderBy(e => e.TimeStamp).Select(o => o.Id).ToList();
        List<string> stringPlaylistIds = [];
        foreach (var playlist in playlistIds) {
            stringPlaylistIds.Add(playlist.ToString());
        }
        await DataController.UpdateKeysAsync(stringPlaylistIds);
    }
    public async void SetQueryType(bool isQueue = true) {
        if (isQueue) {
            _queryType = Common.Value.Data.Playlist_QueueType;
        }
        else {
            _queryType = Common.Value.Data.Playlist_PlaylistType;
        }
        await UpdateData();
    }
    #endregion
}