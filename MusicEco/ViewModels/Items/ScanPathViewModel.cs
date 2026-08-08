using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public sealed partial class ScanPathViewModel: ObservableObject, IUpdateble, ILockableItem {
    public object Identify => this.Path;
    public string Path { get; init; }
    public string Name { get; init; }
    private bool _isLocked;
    public bool IsLocked {
        get => this._isLocked;
        set {
            if (this._isLocked != value) {
                this._isLocked = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsUnlocked));
            }
        }
    }

    public bool IsUnlocked => !IsLocked;

    public ScanPathViewModel(string path) {
        this.Path = path;
        this.Name = System.IO.Path.GetFileName(this.Path) ?? string.Empty;
        this._isLocked = false;
    }
}
