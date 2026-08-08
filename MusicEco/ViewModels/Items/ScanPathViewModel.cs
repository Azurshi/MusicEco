namespace MusicEco.ViewModels.Items;

public sealed class ScanPathViewModel: IUpdateble {
    public object Identify => this.Path;
    public string Path { get; init; }
    public string Name { get; init; }
    public ScanPathViewModel(string path) {
        this.Path = path;
        this.Name = System.IO.Path.GetFileName(this.Path) ?? string.Empty;
    }
}
