using MusicEco.Views.Widgets;
using System.Diagnostics;

namespace MusicEco.Views.Components;
public enum TabListSource {
	Playing,
	Queue,
	Playlist,
	Album,
	Explorer,
	Search,
	User,
	Setting
}
public class TabSwitchedEventArgs(TabListSource value) : EventArgs {
	public TabListSource Value { get; set; } = value;
}
public partial class TabList : ContentView
{
	public event EventHandler<TabSwitchedEventArgs>? TabSwitched;
	private readonly Dictionary<DualStateImage, TabListSource> _buttonList;
	public TabList()
	{
		InitializeComponent();
		this._buttonList = new () {
			{ PlayingButton, TabListSource.Playing},
			{ QueueButton, TabListSource.Queue},
			{ PlaylistButton, TabListSource.Playlist },
			{ AlbumButton, TabListSource.Album },
			{ ExplorerButton, TabListSource.Explorer },
			{ SearchButton, TabListSource.Search },
			{ UserButton, TabListSource.User },
			{ SettingButton, TabListSource.Setting }
		};
		DisableAll();
	}
	private void DisableAll() {
		foreach (var kvp in _buttonList) {
			kvp.Key.IsEnable = false;
		}
	}
	private void Button_Clicked(object sender, EventArgs e) {
        if (sender is DualStateImage image) {
            foreach (var kvp in _buttonList) {
				if (kvp.Key == image) {
					kvp.Key.IsEnable = true;
					Debug.WriteLine($"~~~ TabList {kvp.Value} clicked");
					TabSwitched?.Invoke(this, new(kvp.Value));
				} else {
					kvp.Key.IsEnable = false;
				}
            }
        }
    }
}