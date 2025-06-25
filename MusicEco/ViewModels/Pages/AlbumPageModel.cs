using CommunityToolkit.Mvvm.Input;
using Domain.EventSystem;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels.Pages;
public partial class AlbumPageModel : PropertyObject, IServiceAccess {
    public AlbumPageModel() {
        DataController = new([]);
        EventSystem.Connect<Domain.Events.ScanCommitedEventArgs>(OnScanCommited);
    }
    private readonly ObservableCollectionController<AlbumItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;

    private async void OnScanCommited(object? sender, Domain.Events.ScanCommitedEventArgs args) {
        await LoadData();
    }
    public async Task LoadData() {
        List<string> albumNames = IServiceAccess.DataGetter.AlbumNames().OrderBy(s => s).ToList();
        await DataController.UpdateKeysAsync(albumNames);
        await DataController.PageDown(0, AppSettingModel.Current.GridColumns * AppSettingModel.Current.GridRows);
    }

    [RelayCommand]
    public async Task AlbumSelect(object nameObj) {
        string name = (string)nameObj;
        await Utility.GoToAsync("album_detail", name);
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataGrid.LoadMoreItemEventArgs args) {
        int startIndex = args.ColumnCount * args.LastActivatedRow;
        int amount = args.ColumnCount * args.RowAmount;
        await DataController.PageDown(startIndex, amount);
        //Debug.WriteLine($"{e.ColumnCount} {e.LastActivatedRow} {startIndex} {amount}");
    }
}