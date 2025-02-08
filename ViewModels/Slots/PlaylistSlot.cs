using MusicEco.Models;
using MusicEco.ViewModels.Base;
using System.Diagnostics;

namespace MusicEco.ViewModels.Slots;
public class PlaylistSlot : BaseSlot {
    #region Databinding
    public string? Title { get; private set; }
    #endregion

    protected override Task OnActive() {
        PlaylistModel? model = PlaylistModel.Get(int.Parse(Key));
        if (model != null) {
            Title = model.Name;
        }
        OnPropertyChanged(nameof(Title));
        return Task.CompletedTask;
    }

    public void ChangeName(string name) {
        this.Title = name;
        OnPropertyChanged(nameof(Title));
        PlaylistModel? model = PlaylistModel.Get(int.Parse(Key));
        if (model != null) {
            model.Name = name;
            model.Save();
        }
    }
    public void DeleteSlot() {
        PlaylistModel? model = PlaylistModel.Get(int.Parse(Key));
        model?.Delete();
    }
}