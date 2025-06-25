using Domain.EventSystem;
using Domain.Models;

namespace MusicEco.ViewModels.Components; 
public partial class InfoPreviewModel : PropertyObject, IServiceAccess {
    public string? Title { get; set; }
    public ImageSource? Image { get; set; }
    private static readonly List<string> _propertyNames = [
        nameof(Title), nameof(Image)
        ];
    public InfoPreviewModel() {
        EventSystem.Connect<PlayingSongChangedEventArgs>(
            (s, e) => LoadData()
        );
        LoadData();
    }
    public void LoadData() {
        ISongModel? model = IServiceAccess.ModelGetter.Song(GlobalData.PlayingSongId);
        if (model != null) {
            Title = model.Title;
            Image = IServiceAccess.DataGetter.Image(model);
            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
        }
    }
}
