using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.DetailPages;
public partial class AlbumDetailPageModel : PropertyObject, IQueryAttributable, IServiceAccess {
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.ContainsKey("name")) {
            string name = Uri.UnescapeDataString(Convert.ToString(query["name"]) ?? string.Empty);
            LoadData(name).FireAndForgetAsync();
        }
    }
    private string _albumName = string.Empty;
    public async Task LoadData(string albumName) {
        _albumName = albumName;
        List<ISongModel> albumSongs = IServiceAccess.ModelQuery.Album(albumName, true);
        if (albumSongs.Count > 0) {
            List<string> ids = albumSongs.OrderBy(s => s.Track).Select(s => s.Id.ToString()).ToList();
            await DataController.UpdateKeysAsync(ids);
            await DataController.PageDown(0, AppSettingModel.Current.ListItems);
        }
    }
    public AlbumDetailPageModel() {
        DataController = new([]);
    }
    private readonly ObservableCollectionController<BasicSongItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;

    #region Command
    [RelayCommand]
    public void SongSelect(object keyObj) {
        string key = (string)keyObj;
        long songId = long.Parse(key);
        string queueName = $"Album {_albumName}";
        List<ISongModel> songs = IServiceAccess.ModelQuery.Album(_albumName, true);
        IServiceAccess.PlayQueue(songId, songs, queueName);
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
    #endregion
}
