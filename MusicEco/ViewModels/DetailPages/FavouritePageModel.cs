using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.DetailPages;

public partial class FavouritePageModel: PropertyObject, IServiceAccess {
    public FavouritePageModel() {
        DataController = new([]);
    }
    private readonly ObservableCollectionController<SongItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;

    public async Task LoadData() {
        List<string> songIds = IServiceAccess.ModelQuery.FavouriteSongs()
            .Select(s => s.Id.ToString()).ToList();
        await DataController.UpdateKeysAsync(songIds);
        await DataController.PageDown(0, AppSettingModel.Current.ListItems);
    }
    [RelayCommand]
    public void SongSelect(object keyObj) {
        string key = (string)keyObj;
        long songId = long.Parse(key);
        string queueName = $"Favourite";
        List<ISongModel> songs = IServiceAccess.ModelQuery.FavouriteSongs();
        IServiceAccess.PlayQueue(songId, songs, queueName);
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
}