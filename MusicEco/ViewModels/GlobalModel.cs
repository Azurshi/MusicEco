using CommunityToolkit.Mvvm.Input;
using Domain.EventSystem;
using Domain.Models;
using System.Diagnostics;

namespace MusicEco.ViewModels; 
public partial class GlobalModel: PropertyObject, IServiceAccess {
    [RelayCommand]
    public void Favourite(object keyObj) {
        Debug.WriteLine($"Favourite {keyObj}");
        long songId = 0;
        if (keyObj is string strKeyObj) {
            songId = long.Parse(strKeyObj);
        } else {
            songId = (long)keyObj;
        }
        ISongModel? model = IServiceAccess.ModelGetter.Song(songId);
        if (model != null) {
            model.Favourite = !model.Favourite;
            model.Save();
            EventSystem.Publish<Components.ControlBarModel.FavouriteChangedEventArgs>(this, new(model.Favourite, songId));
        }
    }
}
