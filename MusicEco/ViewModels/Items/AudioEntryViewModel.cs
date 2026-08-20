using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class AudioEntryViewModel: ViewOnlyListItem, IUpdateble {
    public virtual IComparable Identify => (FileHash, DisplayTitle);

    public Hash256 FileHash { get; init; }
    public string DisplayTitle { get; init; }
    public AudioEntryViewModel(Hash256 fileHash, string displayTitle) {
        this.FileHash = fileHash;
        this.DisplayTitle = displayTitle;
    }
}
