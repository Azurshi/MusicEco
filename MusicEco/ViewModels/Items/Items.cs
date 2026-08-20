using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.Views.Buttons;

namespace MusicEco.ViewModels.Items;

public partial class ListItem: ObservableObject, IListItem {
    public static Color EvenColor => DynamicColors.ItemAltBackgroundColor;
    public static Color OddColor => Colors.Transparent;
    [ObservableProperty]
    public partial bool IsActivate { get; set; }
    [ObservableProperty]
    public partial Color BackgroundColor { get; set; }
    public void SetOddBackgroundColor() {
        BackgroundColor = OddColor;
    }
    public void SetEvenBackgroundColor() {
        BackgroundColor = EvenColor;
    }
    public void AutoBackgroundColor(int index) {
        if (index % 2 == 0) {
            BackgroundColor = OddColor;
        }
        else {
            BackgroundColor = EvenColor;
        }
    }
    public ListItem() {
        this.BackgroundColor = EvenColor;
    }
}

public partial class ViewOnlyListItem: ListItem, IEditableListItem, ISelectableItem {
    private bool _listEditing = false;
    public bool ListEditing {
        get => _listEditing;
        set {
            if (_listEditing != value) {
                _listEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ListEditVisibility));
                OnPropertyChanged(nameof(ListViewVisibility));
            }
        }
    }
    public bool ListEditVisibility => _listEditing;
    public bool ListViewVisibility => !_listEditing;
    [ObservableProperty]
    public partial bool Selected { get; set; }
    [ObservableProperty]
    public partial bool IsDraggable { get; set; }
}

public partial class EditableListItem: ViewOnlyListItem, IEditableItem, IMoveableItem {
    private bool _editing = false;
    public bool Editing {
        get => _editing;
        set {
            if (_editing != value) {
                _editing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EditVisibility));
                OnPropertyChanged(nameof(ViewVisibility));
            }
        }
    }
    public bool EditVisibility => _editing;
    public bool ViewVisibility => !_editing;
}