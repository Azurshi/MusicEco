using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Models;
using MusicEco.Views.Widgets;

namespace MusicEco.Views.Components;

public partial class ControlBar : ContentView
{
	private readonly List<VisualElement> _buttons;
	public ControlBar()
	{
		InitializeComponent();
		_buttons = [
			Favourite, Volume, 
			Previous, Backward, Play, Forward, Next, 
			Repeat, Shuffle
		];
        Previous.Rotation = 180;
        Backward.Rotation = 180;
        DisableAll();

		MusicPlayer.PlayStateChanged.Connect(OnMusicPlayer_PlayStateChanged);
        MusicPlayer.ShuffleChanged.Connect(OnMusicPlayer_ShuffleChanged);
        MusicPlayer.RepeatChanged.Connect(OnMusicPlayer_RepeatChanged);

        Play.IsEnable = !MusicPlayer.IsPlaying;
        Shuffle.IsEnable = MusicPlayer.IsShuffle;
        Repeat.IsEnable = MusicPlayer.IsRepeat;

        EventSystem.Connect<IntEventArgs>(Signal.Player_Song_Changed, OnCurrentSongChanged);
	}
    private void OnCurrentSongChanged(object? sender, IntEventArgs e) {
        SongModel? songModel = SongModel.Get(MusicPlayer.LastPlayedId);
        if (songModel != null) {
            Favourite.IsEnable = songModel.Favourite;
        }
    }
	private void OnMusicPlayer_PlayStateChanged(object? sender, EventArgs e) {
		Play.IsEnable = !MusicPlayer.IsPlaying;
	}
    private void OnMusicPlayer_ShuffleChanged(object? sender, BoolEventArgs e) {
        Shuffle.IsEnable = e.Value;
    }
    private void OnMusicPlayer_RepeatChanged(object? sender, BoolEventArgs e) {
        Repeat.IsEnable = e.Value;
    }
	private void DisableAll() {
		foreach (var button in _buttons) {
			if (button is DualStateImage dualStateButton) {
				dualStateButton.IsEnable = false;
			} 
		}
	}
    private void Volume_Clicked(object sender, EventArgs e) {
        EventSystem.Publish(Signal.Overlay_StartChangeAudio, Volume);
    }
    private void Favourite_Clicked(object sender, EventArgs e) {
        SongModel? songModel = SongModel.Get(MusicPlayer.LastPlayedId);
        if (songModel != null) {
            Favourite.IsEnable = !Favourite.IsEnable;
            songModel.Favourite = Favourite.IsEnable;
            songModel.Save();
            EventSystem.Publish<IntEventArgs>(Signal.Song_Favourite_Changed, this, new (songModel.Id));
        }
    }
    private void Shuffle_Clicked(object sender, EventArgs e) {
        //EventSystem.Publish(Common.Events.Signal.Dev_Button1, this);
        MusicPlayer.SetShuffle(!MusicPlayer.IsShuffle);
    }
    private void Repeat_Clicked(object sender, EventArgs e) {
        MusicPlayer.SetRepeat(!MusicPlayer.IsRepeat);
    }
    private async void Play_Clicked(object sender, EventArgs e) {
		await MusicPlayer.PlayPause();
    }
    private async void Previous_Clicked(object sender, EventArgs e) {
        await MusicPlayer.Previous();
    }
    private async void Backward_Clicked(object sender, EventArgs e) {
        await MusicPlayer.Backward();
    }
    private async void Forward_Clicked(object sender, EventArgs e) {
        await MusicPlayer.Forward();
    }
    private async void Next_Clicked(object sender, EventArgs e) {
        await MusicPlayer.Next();
    }
}