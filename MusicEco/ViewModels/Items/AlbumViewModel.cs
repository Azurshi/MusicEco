using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public sealed partial class AlbumViewModel: ListItem, IUpdateble {
    public IComparable Identify { get; init; }
    public string Name { get; init; }
    public IReadOnlyList<Hash256> FileHashes { get; init; }
    public AlbumViewModel(string name, IReadOnlyList<Hash256> fileHashes) {
        this.Name = name;
        this.FileHashes = fileHashes;
        this.Identify = (this.Name, this.FileHashes);
    }
}
