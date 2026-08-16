using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class PlayCountViewModel: AudioEntryViewModel {
    public override object Identify => (FileHash, PlayCount);

    public int PlayCount { get; init; }
    public string PlayCountText => PlayCount.ToString();
    public PlayCountViewModel(Hash256 fileHash, string displayTitle, int playCount) : base(fileHash, displayTitle) {
        this.PlayCount = playCount;
    }
}
