using MusicEco.Common.Events;
using MusicEco.Global;
using System.Diagnostics;

namespace MusicEco.Views.Sections;

public partial class PlayingSection : ContentView
{
    private readonly ViewModels.Sections.PlayingSection ViewModel;
    public PlayingSection() {
        InitializeComponent();
        ViewModel = (ViewModels.Sections.PlayingSection)this.BindingContext;
        EventSystem.Connect<IntEventArgs>(Signal.Player_Song_Changed, OnSongPlayingChanged);

    }
    private void OnSongPlayingChanged(object? sender, IntEventArgs e) {
        ViewModel.ChangeKey(e.Value);
    }
}