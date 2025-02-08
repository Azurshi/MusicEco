using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Models;
using MusicEco.ViewModels.Base;
using System.Diagnostics;

namespace MusicEco.ViewModels.Slots;
public class AdditionSongSlot : SongSlot {
    #region Databinding
    public bool? Favourite { get; private set; }
    public int? Playcount { get; private set; }
    #endregion
    public AdditionSongSlot() {
        EventSystem.Connect<IntEventArgs>(Signal.Song_Favourite_Changed, OnFavouriteOrPlaycount_Changed);
        EventSystem.Connect<IntEventArgs>(Signal.Song_Playcount_Changed, OnFavouriteOrPlaycount_Changed);
    }
    private async void OnFavouriteOrPlaycount_Changed(object? sender, IntEventArgs e) {
        if (e.Value.ToString() == _key) {
            await OnActive();
        }
    }

    private static string[] _propertyNames = [
        nameof(Title), nameof(Icon), nameof(Favourite), nameof(Playcount)
        ];
    protected override Task OnActive() {
        SongModel? model = SongModel.Get(int.Parse(Key));
        if (model != null) {
            Title = model.Title;
            Favourite = model.Favourite;
            Playcount = model.PlayCount;
            if (model.Image != null) Icon = model.Image.Icon;
        }
        foreach (var property in _propertyNames) {
            OnPropertyChanged(property);
        }
        return Task.CompletedTask;
    }
}