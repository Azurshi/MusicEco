using Domain.Models;
using System.Diagnostics;

namespace MusicEco.ViewModels.Items;
public partial class BasicSongItemModel : BaseItem, IServiceAccess {
    public ImageSource? Icon { get; set; }
    public string? Title { get; set; }
    public bool Available { get; set; }
    private static readonly List<string> _propertyNames = [
        nameof(Key), nameof(Title), nameof(Icon), nameof(Available)
        ];
    protected override async Task OnActive() {
        if (Key == string.Empty) return;
        ISongModel? model = IServiceAccess.ModelGetter.Song(long.Parse(Key));
        if (model != null) {
            Available = model.Available;
            Title = model.Title;
            Icon = IServiceAccess.DataGetter.Icon(model);
            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
            await Task.CompletedTask;
        }
    }
}
