using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public sealed class QueueItemViewModel: ObservableObject, IUpdateble, ISelectableItem {
    public object Identify => (CreationTime, ModifiedTime);
    public DateTime CreationTime { get; init; }
    public DateTime ModifiedTime { get; init; }
    public string Name { get; init; }
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
    public QueueItemViewModel(DateTime creationTime, DateTime modifiedTime, string name) {
        this.CreationTime = creationTime;
        this.ModifiedTime = modifiedTime;
        this.Name = name;
    }
}
