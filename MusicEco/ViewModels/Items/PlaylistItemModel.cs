using Domain.Models;

namespace MusicEco.ViewModels.Items;
public partial class PlaylistItemModel: BaseModifiableDraggableItem, IServiceAccess {
    public string? Title { get; set; }
    private static readonly List<string> _propertyNames = [
        nameof(Key), nameof(Title)
        ];
    protected override async Task OnActive() {
        if (Key == string.Empty) return;
        IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(long.Parse(Key));
        if (playlistModel != null) {
            Title = playlistModel.Name;
            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
            await Task.CompletedTask;
        }
    }
    public override Task SaveData() {
        IPlaylistModel? queueModel = IServiceAccess.ModelGetter.Playlist(long.Parse(Key));
        if (queueModel != null) {
            if (Title != null) {
                queueModel.Name = Title;
                queueModel.Save();
                foreach (var propertyName in _propertyNames) {
                    OnPropertyChanged(propertyName);
                }
            }
        }
        return Task.CompletedTask;
    }
}
