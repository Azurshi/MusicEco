using DataStorage.Models;
using Domain.Models;

namespace MusicEco.ViewModels.Items;
public partial class SongItemModel : BaseDraggableItem, IServiceAccess {
    public ImageSource? Icon { get; set; }
    public string? Title { get; set; }
    public int Playcount { get; set; } = 0;
    private bool isFavourite = false;
    public bool IsFavourite {
        get => isFavourite;
        set {
            isFavourite = value;
            ISongModel? songModel = IServiceAccess.ModelGetter.Song(long.Parse(Key));
            if (songModel != null) {
                songModel.Favourite = value;
                songModel.Save();
            }
        }
    }
    public bool Available { get; set; }
    private static readonly List<string> _propertyNames = [
        nameof(Key), nameof(Title), nameof(Icon), nameof(Playcount), nameof(IsFavourite), nameof(Available)
        ];
    protected override async Task OnActive() {
        if (Key == string.Empty) return;
        ISongModel? model = IServiceAccess.ModelGetter.Song(long.Parse(Key));
        if (model != null) {
            Title = model.Title;
            Playcount = model.PlayCount;
            IsFavourite = model.Favourite;
            Available = model.Available;
            Icon = IServiceAccess.DataGetter.Icon(model);
            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
            await Task.CompletedTask;
        }
    }
}