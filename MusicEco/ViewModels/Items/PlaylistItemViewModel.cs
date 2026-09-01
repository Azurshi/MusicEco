using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class PlaylistItemViewModel: ListItem, IUpdateble {
    public IComparable Identify => (CreationTime, ModifiedTime);
    public DateTime CreationTime { get; init; }
    public DateTime ModifiedTime { get; init; }
    public string Name { get; init; }
    public PlaylistItemViewModel(DateTime creationTime, DateTime modifiedTime, string name) {
        this.CreationTime = creationTime;
        this.ModifiedTime = modifiedTime;
        this.Name = name;
    }
}