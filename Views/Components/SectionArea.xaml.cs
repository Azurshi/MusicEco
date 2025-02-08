namespace MusicEco.Views.Components;

public partial class SectionArea : ContentView
{
	private readonly List<VisualElement> _sections;
	public SectionArea()
	{
		InitializeComponent();
		this._sections = [
			Playing, Queue, Playlist, Album, 
			Explorer, Search, User, Setting
		];
		DisableAll();
		EnablePlaying();
	}
	private void DisableAll() {
		foreach(var section in _sections) {
			section.IsEnabled = false;
			section.IsVisible = false;
		}
	}
	public void On_TabList_Switched(object? sender, TabSwitchedEventArgs e) {
		DisableAll();
		switch (e.Value) {
			case TabListSource.Playing: 
				this.EnablePlaying(); 
				break;
			case TabListSource.Queue:
				this.EnableQueue();
				break;
			case TabListSource.Playlist:
				this.EnablePlaylist();
				break;
			case TabListSource.Album:
				this.EnableAlbum();
				break;
			case TabListSource.Explorer:
				this.EnableExplorer();
				break;
			case TabListSource.Search:
				this.EnableSearch();
				break;
			case TabListSource.User:
				this.EnableUser();
				break;
			case TabListSource.Setting:
				this.EnableSetting();
				break;
			default:
				throw new NotImplementedException();
		}
	}
	private void EnablePlayingAsSub() {
#if WINDOWS
		Playing.IsEnabled = true;
        Playing.IsVisible = true;
        Grid.SetColumnSpan(Playing, 1);
#endif
	}
	private void EnablePlaying() {
		Playing.IsEnabled = true;
		Playing.IsVisible = true;
		Grid.SetColumn(Playing, 0);
		Grid.SetColumnSpan(Playing, 2);
	}
	private void EnableQueue() {
		EnablePlayingAsSub();
        Queue.IsEnabled = true;
        Queue.IsVisible = true;
		Grid.SetColumn(Queue, 1);
    }
	private void EnablePlaylist() {
		EnablePlayingAsSub();
		Playlist.IsEnabled = true;
		Playlist.IsVisible = true;
		Grid.SetColumn(Playlist, 1);
	}
	private void EnableAlbum() {
		EnablePlayingAsSub();
        Album.IsEnabled = true;
        Album.IsVisible = true;
        Grid.SetColumn(Album, 1);
    }
	private void EnableExplorer() {
		EnablePlayingAsSub();
		Explorer.IsEnabled = true;
		Explorer.IsVisible = true;
		Grid.SetColumn(Explorer, 1);
	}
	private void EnableSearch() {
		EnablePlayingAsSub();
		Search.IsEnabled = true;
		Search.IsVisible = true;
		Grid.SetColumn(Search, 1);
	}
	private void EnableUser() {
		EnablePlayingAsSub();
		User.IsEnabled = true;
		User.IsVisible = true;
		Grid.SetColumn(User, 1);
	}
	private void EnableSetting() {
		Setting.IsEnabled = true;
		Setting.IsVisible = true;
		Grid.SetColumn(Setting, 0);
		Grid.SetColumnSpan(Setting, 2);
	}
}