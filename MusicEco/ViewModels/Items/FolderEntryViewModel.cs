using MusicEco.Core.Types;
namespace MusicEco.ViewModels.Items;

public partial class FolderEntryViewModel: ListItem, IUpdateble {
    public IComparable Identify => this.Path;
    public string Path { get; init; }
    public string Name { get; init; }
    public FolderEntryViewModel(string path) {
        this.Path = path;
        this.Name = System.IO.Path.GetFileName(this.Path) ?? string.Empty;
    }
}
