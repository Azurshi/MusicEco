using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Models;
using System.Diagnostics;

namespace MusicEco.ViewModels.Sections;
public class PlayingSection: PropertyObject {
    #region Databinding
    public int TitleSlotHeight => Common.Value.UI.RowHeight * 2;
    public string? Title { get; private set; }
    public ImageSource? Image { get; private set; }
    #endregion

    public PlayingSection() {
        EventSystem.Connect<IntEventArgs>(Signal.UI_RowHeight_Changed, OnRowHeightChanged);
    }
    private static readonly string[] _propertyNames = [
        nameof(Title), nameof(Image)
    ];
    public void ChangeKey(int songId) {
        SongModel? model = SongModel.Get(songId);
        if (model != null) {
            Title = model.Title;
            Image = model.Image?.Source;
        }
        foreach (var property in _propertyNames) {
            OnPropertyChanged(property);
        }
    }
    private void OnRowHeightChanged(object? sender, IntEventArgs e) {
        OnPropertyChanged(nameof(TitleSlotHeight));
    }
}
