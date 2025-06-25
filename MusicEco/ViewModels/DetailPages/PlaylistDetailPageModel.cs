using CommunityToolkit.Mvvm.Input;
using DataStorage.DataAccess;
using Domain.EventSystem;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels.DetailPages;
public partial class PlaylistDetailPageModel : DragList, IQueryAttributable, IServiceAccess {
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.ContainsKey("id")) {
            long id = Convert.ToInt64(query["id"]);
            LoadData(id).FireAndForgetAsync();
        }
    }
    private long _playlistId = -1;
    public async Task LoadData(long playlistId) {
        _playlistId = playlistId;
        IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(playlistId);
        if (playlistModel != null) {
            List<string> ids = playlistModel.Songs.Select(s => s.Id.ToString()).ToList();
            await DataController.UpdateKeysAsync(ids);
            await DataController.PageDown(0, AppSettingModel.Current.ListItems);
        }
    }
    public override async Task OnDropCompleted(int index, string key) {
        IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(_playlistId);
        if (playlistModel != null) {
            long id = long.Parse(key);
            ISongModel? target = playlistModel.Songs.Where(s => s.Id == id).FirstOrDefault();
            if (target != null) {
                playlistModel.RemoveSong(target);
                playlistModel.InsertSong(index, target);
                playlistModel.Save();
            }
            else {
                Debug.WriteLine($"Target is null {key}");
            }
            await LoadData(_playlistId);
        }
    }
    public PlaylistDetailPageModel() {
        DataController = new([]);
        EventSystem.Connect<MusicPlayer.ShuffledEventArgs>(OnShuffled);
    }
    private async void OnShuffled(object? sender, MusicPlayer.ShuffledEventArgs e) {
        if (_playlistId == e.QueueId) {
            IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(e.QueueId);
            if (playlistModel != null && playlistModel.Type == DefaultValue.Queue) {
                await LoadData(playlistModel.Id);
            }
        }
    }
    private readonly ObservableCollectionController<SongItemModel> DataController;
    public override ObservableCollection<BaseItem> Data => DataController.Target;

    /// <summary>
    /// Fix invalid queue when rename playlist
    /// </summary>
    /// <param name="keyObj"></param>
    [RelayCommand]
    public void SongSelect(object keyObj) {
        string key = (string)keyObj;
        long songId = long.Parse(key);
        //Debug.WriteLine($"Playlist select {songId}");
        IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(_playlistId);
        if (playlistModel == null) return;
        if (playlistModel.Type == DefaultValue.Queue) {
            playlistModel.Current = IServiceAccess.ModelGetter.Song(songId);
            playlistModel.Save();
            GlobalData.PlayingQueueId = playlistModel.Id;
        }
        else {
            string playlistName = $"Playlist {playlistModel.Name}";
            IServiceAccess.PlayQueue(songId, (List<ISongModel>)playlistModel.Songs, playlistName);
        }
    }
    [RelayCommand]
    private async Task SongRemove(object keyObj) {
        string key = (string)keyObj;
        long songId = long.Parse(key);
        IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(_playlistId);
        if (playlistModel != null) {
            ISongModel? song = IServiceAccess.ModelGetter.Song(songId);
            if (song != null) {
                playlistModel.RemoveSong(song); // Should be handled remove current in model func
                playlistModel.Save();
                await LoadData(playlistModel.Id);
            }
        }
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
}
