using MusicEco.Core.Types;
namespace MusicEco.ViewModels;


public interface ISelectableItem {
    public bool Selected { get; set; }
}
public interface IEditableListItem {
    public bool ListEditing { get; set; }
    public bool ListEditVisibility { get; }
    public bool ListViewVisibility { get; }
}
public interface IListItem: IBackgroundItem {
    public Color BackgroundColor { get; }
}
public interface IEditableItem {
    public bool Editing { get; set; }
    public bool EditVisibility { get; }
    public bool ViewVisibility { get; }
}
public interface ILockableItem {
    public bool IsLocked { get; set; }
    public bool IsUnlocked { get; }
}
public interface IActivableItem {
    public bool IsActivated { get; set; }
}

public enum CollectionDisplayMode {
    None,
    SimpleGrid,
    SimpleList,
    DetailList
}

public interface IAnimationAware {
    public bool IsVisibleInViewport { get; set; }
}