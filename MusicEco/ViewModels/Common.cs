namespace MusicEco.ViewModels;
[Flags]
public enum ViewMode {
    None = 0,
    Confirm = 1,
    Cancel = 2,
    View = Confirm | Cancel,
    Edit = 4,
    Default = View
}

public interface IDraggable {
    public bool IsDraggable { get; set; }
    public bool IsDragged { get; set; }
    public bool IsDraggedOver { get; set; }

    public string Key { get; }
}