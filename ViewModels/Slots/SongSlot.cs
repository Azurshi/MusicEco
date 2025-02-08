using MusicEco.Common;
using MusicEco.Models;
using MusicEco.ViewModels.Base;

namespace MusicEco.ViewModels.Slots;
public class SongSlot : BaseSlot {
    #region Databinding
    public string? Title { get; protected set; }
    public ImageSource? Icon { get; protected set; }
    #endregion

    protected override Task OnActive() {
        SongModel? model = SongModel.Get(int.Parse(Key));
        if (model != null) {
            Title = model.Title;
            if (model.Image != null) Icon = model.Image.Icon;
        }
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Icon));
        return Task.CompletedTask;
    }


    public void AddToPlaylist(int playlistId) {
        PlaylistModel? model = PlaylistModel.Get(playlistId);
        if (model != null) {
            model.SongIds.Add(int.Parse(this.Key));
            model.Save();
        }
    }
    public void AddToQueue(int queueId) { 
        AddToPlaylist(queueId);
    }
    public void DeleteFromCurrentSelectedPlaylist() {
        PlaylistModel? model = PlaylistModel.Get(UserData.CurrentSelectedPlaylist);
        if (_key != null && model != null) {
            model.SongIds.Remove(int.Parse(_key));
            model.Save();
        }
    }
    public void DeleteFromCurrentSelectedQueue() {
        PlaylistModel? model = PlaylistModel.Get(UserData.CurrentSelectedQueue);
        if (_key != null && model != null) {
            model.SongIds.Remove(int.Parse(_key));
            model.Save();
        }
    }
    public void DeleteFromCurrentPlayingQueue() {
        PlaylistModel? model = PlaylistModel.Get(UserData.CurrentPlayingQueue);
        if (_key != null && model != null) {
            model.SongIds.Remove(int.Parse(_key));
            model.Save();
        }
    }
}