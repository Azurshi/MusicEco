using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels.Pages; 
public partial class QueuePageModel : DragList, IServiceAccess {
    public QueuePageModel() {
        DataController = new([]);
    }
    private readonly ObservableCollectionController<PlaylistItemModel> DataController;
    public override ObservableCollection<BaseItem> Data => DataController.Target;
    private string query = string.Empty;
    private bool movable = true;
    public bool Movable {
        get => movable;
        set {
            movable = value;
            OnPropertyChanged(nameof(Movable));
        }
    }
    public string Query {
        get => query;
        set {
            query = value;
            LoadData().FireAndForgetAsync();
            OnPropertyChanged(nameof(Query));
        }
    }
    public async Task LoadData() {
        List<string> queueIds = [];
        if (query.Length > 3) {
            queueIds = IServiceAccess.ModelGetter.PlaylistList()
                .Where(s => s.Type == DefaultValue.Queue)
                .Where(s => s.Name.Contains(this.query, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(s => s.Order)
                .Select(s => s.Id.ToString()).ToList();
            Movable = false;
        } else {
            queueIds = IServiceAccess.ModelGetter.PlaylistList()
                .Where(s => s.Type == DefaultValue.Queue)
                .OrderByDescending(s => s.Order)
                .Select(s => s.Id.ToString()).ToList();
            Movable = true;
        }

        await DataController.UpdateKeysAsync(queueIds);
        await DataController.PageDown(0, AppSettingModel.Current.ListItems);
    }
    public override async Task OnDropCompleted(int index, string key) {
        List<IPlaylistModel> queues = IServiceAccess.ModelGetter.PlaylistList()
            .Where(s => s.Type == DefaultValue.Queue)
            .OrderByDescending(s => s.Order).ToList();
        long id = long.Parse(key);
        IPlaylistModel? target = queues.Where(s => s.Id == id).FirstOrDefault();
        if (target != null) {
            queues.Remove(target);
            queues.Insert(index, target);
            for (int i = 0; i < queues.Count; i++) {
                queues[i].Order = i;
                queues[i].Save();
            }
        }
        await Task.CompletedTask;
    }
    [RelayCommand]
    public async Task QueueSelect(object idObj) {
        long id = long.Parse((string)idObj);
        Debug.WriteLine(id);
        await Utility.GoToAsync("playlist_detail", id);
    }
    [RelayCommand]
    public async Task QueueDelete(object idObj) {
        long id = long.Parse((string)idObj);
        Debug.WriteLine(id);
        IPlaylistModel? queueModel = IServiceAccess.ModelGetter.Playlist(id);
        if (queueModel != null) {
            queueModel.Delete();
            await LoadData();
        }
    }
    [RelayCommand]
    private void QueueOptionMenu(object idObj) {
        long id = long.Parse((string)idObj);
        Debug.WriteLine(id);
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
}
