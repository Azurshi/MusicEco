namespace MusicEco.ViewModels.Items;
public class ScanItemModel : BaseItem {
    public string? FolderName { get; set; }
    public string? FolderPath { get; set; }
    private static readonly List<string> _propertyNames = [
        nameof(Key), nameof(FolderName), nameof(FolderPath)
    ];
    protected override Task OnActive() {
        if (Key == string.Empty) return Task.CompletedTask;
        int index = int.Parse(Key);
        string folderPath = GlobalData.ScanFolders[index];
        DirectoryInfo directoryInfo = new(folderPath);
        FolderPath = directoryInfo.FullName;
        FolderName = directoryInfo.Name;
        foreach (var propertyName in _propertyNames) {
            OnPropertyChanged(propertyName);
        }
        return Task.CompletedTask;
    }
}
