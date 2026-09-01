using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class FileEntryViewModel: ListItem, IUpdateble {
    public IComparable Identify => (this.FileHash, this.Path);
    public Hash256 FileHash { get; init; }
    public string Path { get; init; }
    public string Name { get; init; }
#if ANDROID
    public FileEntryViewModel(Hash256 fileHash, string path, string name) {
        this.FileHash = fileHash;
        this.Path = path;
        this.Name = name;
    }
#else
    public FileEntryViewModel(Hash256 fileHash, string path) {
        this.FileHash = fileHash;
        this.Path = path;
        this.Name = System.IO.Path.GetFileName(this.Path) ?? string.Empty;
    }
#endif
}