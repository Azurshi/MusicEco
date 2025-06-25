using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.Pages;
public partial class PlaylistPageModel : PropertyObject, IServiceAccess {
    public PlaylistPageModel() {
        DataController = new([]);
    }
    private readonly ObservableCollectionController<PlaylistItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;
    public async Task LoadData() {
        List<string> playlistIds = IServiceAccess.ModelGetter.PlaylistList()
            .Where(s => s.Type == DefaultValue.Playlist)
            .OrderBy(s => s.Name)
            .Select(s => s.Id.ToString()).ToList();
        await DataController.UpdateKeysAsync(playlistIds);
        await DataController.PageDown(0, AppSettingModel.Current.ListItems);
    }

    [RelayCommand]
    public async Task AddPlaylist() {
        IPlaylistModel playlistModel = IServiceAccess.Service.GetRequiredService<IPlaylistModel>();
        playlistModel.Type = DefaultValue.Playlist;
        playlistModel.AssignId();
        playlistModel.Name = "New playlist";
        playlistModel.Save();
        playlistModel.ConsolideOrder(playlistModel.Type);
        await LoadData();
    }
    [RelayCommand]
    public async Task PlaylistSelect(string strId) {
        long id = long.Parse(strId);
        await Utility.GoToAsync("playlist_detail", id);
    }
    [RelayCommand]
    public async Task PlaylistDelete(string strId) {
        long id = long.Parse(strId);
        IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(id);
        if (playlistModel != null) {
            playlistModel.Delete();
            await LoadData();
        }
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
}