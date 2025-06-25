using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.DetailPages;

public partial class PlaycountPageModel: PropertyObject, IServiceAccess {
    public PlaycountPageModel() {
        DataController = new([]);
    }
    private readonly ObservableCollectionController<SongItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;

    public async Task LoadData() {
        List<string> songIds = IServiceAccess.ModelQuery.SongByPlaycount(1)
            .Select(s => s.Id.ToString())
            .Reverse().ToList();
        await DataController.UpdateKeysAsync(songIds);
        await DataController.PageDown(0, AppSettingModel.Current.ListItems);
    }
    [RelayCommand]
    public void SongSelect(object keyObj) {
        string key = (string)keyObj;
        long songId = long.Parse(key);
        string queueName = $"Playcount";
        List<ISongModel> songs = IServiceAccess.ModelQuery.SongByPlaycount(1);
        IServiceAccess.PlayQueue(songId, songs, queueName);
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
}