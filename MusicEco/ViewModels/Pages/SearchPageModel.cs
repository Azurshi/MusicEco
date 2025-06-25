using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.Pages;
public partial class SearchPageModel : PropertyObject, IQueryAttributable, IServiceAccess {
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.ContainsKey("name")) {
            string name = Uri.UnescapeDataString(Convert.ToString(query["name"]) ?? string.Empty);
            LoadData(name).FireAndForgetAsync();
        }
    }
    private string _nameQuery = string.Empty;
    public string NameQuery {
        get => _nameQuery;
        set {
            _nameQuery = value;
            OnPropertyChanged();
            LoadData().FireAndForgetAsync();
        }
    }
    public async Task LoadData(string nameQuery) {
        NameQuery = nameQuery;
        await Task.CompletedTask;
    }
    private async Task LoadData() {
        if (_nameQuery.Length < 3) return;
        List<ISongModel> albumSongs = IServiceAccess.ModelQuery.Song(_nameQuery);
        if (albumSongs.Count > 0) {
            List<string> ids = albumSongs.OrderBy(s => s.Track).Select(s => s.Id.ToString()).ToList();
            await DataController.UpdateKeysAsync(ids);
            await DataController.PageDown(0, AppSettingModel.Current.ListItems);
        }
    }
    public SearchPageModel() {
        DataController = new([]);
    }
    private readonly ObservableCollectionController<BasicSongItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;

    [RelayCommand]
    public void SongSelect(object keyObj) {
        string key = (string)keyObj;
        long songId = long.Parse(key);
        string queueName = $"Search {_nameQuery}";
        List<ISongModel> songs = IServiceAccess.ModelQuery.Song(_nameQuery);
        IServiceAccess.PlayQueue(songId, songs, queueName);
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
}
