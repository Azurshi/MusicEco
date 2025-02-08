using MusicEco.Models;
using MusicEco.ViewModels.Base;

namespace MusicEco.ViewModels.Slots;
public class AlbumOverviewSlot: BaseSlot {
    #region Databinding
    public new int SlotHeight => base.SlotHeight * 3;
    public string? Title { get; private set; }
    public ImageSource? Icon { get; private set; }
    #endregion

    protected override Task OnActive() {
        SongModel? model = SongModel.GetByAlbum(Key);
        if (model != null) {
            Title = model.Album;
            if (model.Image != null) Icon = model.Image.Icon;
        }
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Icon));
        return Task.CompletedTask;
    }
}