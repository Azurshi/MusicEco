using MusicEco.Core.Types;
namespace MusicEco.ViewModels.Items;

public partial class FolderEntryViewModel: ListItem, IUpdateble {
    public IComparable Identify => this.Path;
    public string Path { get; init; }
    public string Name { get; init; }
#if ANDROID
    public Android.Net.Uri Uri { get; init; }
    public FolderEntryViewModel(string path, string name, Android.Net.Uri uri) {
        this.Path = path;
        this.Name = name;
        this.Uri = uri;
    }
#else
    public FolderEntryViewModel(string path) {
        this.Path = path;
        this.Name = System.IO.Path.GetFileName(this.Path) ?? string.Empty;
    }
#endif
}
