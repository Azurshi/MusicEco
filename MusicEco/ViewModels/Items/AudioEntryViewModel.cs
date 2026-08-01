using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public sealed class AudioEntryViewModel {
    public Hash256 FileHash { get; init; }
    public string DisplayTitle { get; init; }
    public AudioEntryViewModel(Hash256 fileHash, string displayTitle) {
        this.FileHash = fileHash;
        this.DisplayTitle = displayTitle;
    }
}
