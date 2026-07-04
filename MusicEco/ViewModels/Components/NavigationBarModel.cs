using CommunityToolkit.Mvvm.Input;
using Domain.EventSystem;
using System.Diagnostics;

namespace MusicEco.ViewModels.Components;

public partial class NavigationBarModel : PropertyObject {
    private readonly Dictionary<string, ImageState> imageStates = [];
    private void Initialize() {
        imageStates[nameof(OverviewImage)] = new("playing_on.png", "playing.png");
        imageStates[nameof(QueueImage)] = new("playlist_on.png", "playlist.png");
        imageStates[nameof(PlaylistImage)] = new("playlist_on.png", "playlist.png");
        imageStates[nameof(AlbumImage)] = new("album_on.png", "album.png");
        imageStates[nameof(ExplorerImage)] = new("folder_on.png", "folder.png");
        imageStates[nameof(SearchImage)] = new("search_on.png", "search.png");
        imageStates[nameof(UserImage)] = new("user_on.png", "user.png");
        imageStates[nameof(SettingImage)] = new("setting_on.png", "setting.png");
    }
    private void SetAllOff() {
        foreach (var kvp in imageStates) {
            kvp.Value.IsOn = false;
            //OnPropertyChanged(kvp.Key);
        }
    }
    private void SetTrue(string key) {
        imageStates[key].IsOn = true;
        //OnPropertyChanged(key);
    }
    private void Switch(string key) {
        SetAllOff();
        SetTrue(key);
    }
    public NavigationBarModel() {
        Initialize();
    }
    public ImageSource OverviewImage => imageStates[nameof(OverviewImage)].Source;
    public ImageSource QueueImage => imageStates[nameof(QueueImage)].Source;
    public ImageSource PlaylistImage => imageStates[nameof(PlaylistImage)].Source;
    public ImageSource AlbumImage => imageStates[nameof(AlbumImage)].Source;
    public ImageSource ExplorerImage => imageStates[nameof(ExplorerImage)].Source;
    public ImageSource SearchImage => imageStates[nameof(SearchImage)].Source;
    public ImageSource UserImage => imageStates[nameof(UserImage)].Source;
    public ImageSource SettingImage => imageStates[nameof(SettingImage)].Source;
    public void DisplayAsPlaylist() {
        Switch(nameof(PlaylistImage));
    }
    public void DisplayAsQueue() {
        Switch(nameof(QueueImage));
    }
    [RelayCommand]
    public async Task NavigateOverview() {
        Switch(nameof(OverviewImage));
        Debug.WriteLine("Navigate to overview");
        await Navigator.GoToAsync("overview", GlobalData.PlayingSongId);

    }
    [RelayCommand]
    public async Task NavigateQueue() {
        Switch(nameof(QueueImage));
        Debug.WriteLine("Navigae to queue");
        await Navigator.GoToAsync("queue");
    }
    [RelayCommand]
    public async Task NavigatePlaylist() {
        Switch(nameof(PlaylistImage));
        Debug.WriteLine("Navigate to playlist");
        await Navigator.GoToAsync("playlist");
    }
    [RelayCommand]
    public async Task NavigateAlbum() {
        Switch(nameof(AlbumImage));
        Debug.WriteLine("Navigate to album");
        await Navigator.GoToAsync("album");
    }
    [RelayCommand]
    public async Task NavigateExplorer() {
        Switch(nameof(ExplorerImage));
        Debug.WriteLine("Navigate to explorer");
        long folderId = GlobalData.CurrentFolderId ?? -1;
        await Navigator.GoToAsync("explorer", folderId);
    }
    [RelayCommand]
    public async Task NavigateSearch() {
        Switch(nameof(SearchImage));
        Debug.WriteLine("Navigate to search");
        await Navigator.GoToAsync("search", "");
    }
    [RelayCommand]
    public async Task NavigateUser() {
        Switch(nameof(UserImage));
        Debug.WriteLine("Navigate to user");
        await Navigator.GoToAsync("user");
    }
    [RelayCommand]
    public async Task NavigateSetting() {
        Switch(nameof(SettingImage));
        Debug.WriteLine("Navigate setting");
        await Navigator.GoToAsync("setting");
    }
}
