using Domain.Models;

namespace MusicEco.ViewModels.Items;
public partial class AlbumItemModel : BaseItem, IServiceAccess {
    
    public ImageSource? Icon { get; set; }
    public string? Title { get; set; }
    private static readonly List<string> _propertyNames = [
        nameof(Key), nameof(Icon), nameof(Title)
        ];
    public AlbumItemModel() {
#if WINDOWS // Tempory fix to improve perf on Android
        RefreshOnKeyChanged = true;
#endif
    }
    protected override async Task OnActive() {
        if (Key == string.Empty) return;
        List<ISongModel> albumSongs = IServiceAccess.ModelQuery.Album(Key, true);
        if (albumSongs.Count > 0) {
            ISongModel firstSong = albumSongs[0];
            Title = Key;
            Icon = IServiceAccess.DataGetter.Icon(firstSong);
            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
            await Task.CompletedTask;
        }
    }
}
