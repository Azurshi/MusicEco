using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.Components; 
public partial class PlaylistSelectionListModel: PropertyObject, IServiceAccess {
    protected string Type = DefaultValue.Playlist;
    protected long targetSongId = -1;
    public long TargetSongId {
        get => targetSongId;
        set => targetSongId = value;
    }
    public VoidEventHandler? CleaupFunction;
    public PlaylistSelectionListModel() {
        DataController = new([]);
        Type = DefaultValue.Playlist;
    }
    protected readonly ObservableCollectionController<PlaylistSelectionItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;
    public async Task LoadData() {
        string targetSongString = targetSongId.ToString();
        List<string> queueIds = IServiceAccess.ModelGetter.PlaylistList()
            .Where(s => s.Type == Type)
            .OrderBy(s => s.Order)
            .Select(s => s.Id.ToString() + "_" + targetSongString).ToList();
        await DataController.UpdateKeysAsync(queueIds);
        await DataController.PageDown(0, AppSettingModel.Current.ListItems);
    }
    public static long ExtractId(object sender) {
        return long.Parse(((BaseItem)((VisualElement)sender).BindingContext).Key);
    }
    public static long ConvertFromFileIdToSongId(long fileId) {
        ISongModel? song = IServiceAccess.ModelQuery.SongByFileId(fileId);
        if (song != null) {
            return song.Id;
        }
        else {
            return -1;
        }
    }
    [RelayCommand]
    public void ItemSelect(object keyObj) {
        string key = (string)keyObj;
        long playlistId = long.Parse(key.Split("_")[0]);
        long songId = long.Parse(key.Split("_")[1]);
        IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(playlistId);
        ISongModel? song = IServiceAccess.ModelGetter.Song(songId);
        if (playlistModel != null && song != null) {
            playlistModel.AddSong(song);
            playlistModel.Save();
        }
        CleaupFunction?.Invoke();
        CleaupFunction = null;
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
}

public partial class PlaylistSelectionItemModel: BaseItem, IServiceAccess {
    public string? Title { get; set; }
    public bool IsEnable { get; set; } = true;

    private static readonly List<string> _propertyNames = [
        nameof(Key), nameof(Title), nameof(IsEnable)
        ];
    protected override async Task OnActive() {
        if (Key == string.Empty) return;
        long targetSongId = long.Parse(Key.Split('_')[1]);
        long songId = long.Parse(Key.Split("_")[0]);
        IPlaylistModel? queueModel = IServiceAccess.ModelGetter.Playlist(songId);
        if (queueModel != null) {
            Title = queueModel.Name;
            IReadOnlyList<ISongModel> songs = queueModel.Songs;
            IsEnable = true;
            foreach(ISongModel song in songs) {
                if (song.Id == targetSongId) {
                    IsEnable = false;
                    break;
                }
            }
            //Debug.WriteLine($"{IsEnable} {targetSongId} {songId}");

            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
            await Task.CompletedTask;
        }
    }
}

