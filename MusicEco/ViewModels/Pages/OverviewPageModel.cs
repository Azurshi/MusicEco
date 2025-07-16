using Domain.EventSystem;
using Domain.Models;

namespace MusicEco.ViewModels.Pages;
public partial class OverviewPageModel: PropertyObject, IQueryAttributable, IServiceAccess {
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.ContainsKey("id")) {
            long songId = Convert.ToInt64(query["id"]);
            LoadData(songId);
        }
    }
    public string? Title { get; set; }
    public ImageSource? Image { get; set; }
    private static readonly List<string> _propertyNames = [
        nameof(Title), nameof(Image)
        ];
    public OverviewPageModel() {
        EventSystem.Connect<PlayingSongChangedEventArgs>(
            (s, e) => LoadData(e.SongId)
        );
        LoadData(GlobalData.PlayingSongId);
    }
    public async void LoadData(long id) {
        ISongModel? model = IServiceAccess.ModelGetter.Song(id);
        if (model != null) {
            if (model.Title != string.Empty) {
                Title = model.Title;
            }
            else {
                Title = model.File?.Name;
            }
            Image = await IServiceAccess.DataGetter.Image(model);
            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
        }
    }
}
