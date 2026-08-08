using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public sealed class AlbumViewModel: IUpdateble {
    public object Identify { get; init; }
    public string Name { get; init; }
    public IReadOnlyList<Hash256> FileHashes { get; init; }
    public AlbumViewModel(string name, IReadOnlyList<Hash256> fileHashes) {
        this.Name = name;
        this.FileHashes = fileHashes;
        string itendify = this.Name;
        foreach(var hash in fileHashes) {
            itendify += ":" + Convert.ToHexString(hash.AsReadOnlySpan());
        }
        this.Identify = itendify;
    }
}
