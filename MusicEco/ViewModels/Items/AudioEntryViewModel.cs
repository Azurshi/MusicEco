using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class AudioEntryViewModel: ObservableObject, IUpdateble, ISelectableItem {
    public object Identify => FileHash;

    public Hash256 FileHash { get; init; }
    public string DisplayTitle { get; init; }
    private bool _selected = false;
    public bool Selected {
        get => this._selected;
        set {
            if (this._selected != value) {
                this._selected = value;
                OnPropertyChanged();
            }
        }
    }
    public AudioEntryViewModel(Hash256 fileHash, string displayTitle) {
        this.FileHash = fileHash;
        this.DisplayTitle = displayTitle;
    }
}
