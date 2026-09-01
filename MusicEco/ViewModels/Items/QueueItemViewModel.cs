using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class QueueItemViewModel: ViewOnlyListItem, IUpdateble {
    public IComparable Identify => (CreationTime, ModifiedTime);
    public DateTime CreationTime { get; init; }
    public DateTime ModifiedTime { get; init; }
    public DateTime LastPlayTime { get; init; }
    public string Name { get; init; }
    public QueueItemViewModel(DateTime creationTime, DateTime modifiedTime, DateTime lastPlayTime, string name) {
        this.CreationTime = creationTime;
        this.ModifiedTime = modifiedTime;
        this.LastPlayTime = lastPlayTime;
        this.Name = name;
    }
}
