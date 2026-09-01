using MusicEco.ViewModels;

namespace MusicEco.Views.Controls;

public class CollectionViewPreset {
    public CollectionDisplayMode DisplayMode { get; set; }
    public ItemsLayout? ItemsLayout { get; set; }
    public DataTemplate? ItemTemplate { get; set; }
    public CollectionViewPreset() {
        this.DisplayMode = CollectionDisplayMode.SimpleList;
        this.ItemsLayout = null;
        this.ItemTemplate = null;
    }
    public CollectionViewPreset(CollectionDisplayMode displayMode, ItemsLayout itemsLayout, DataTemplate? itemTemplate) {
        this.DisplayMode = displayMode;
        this.ItemsLayout = itemsLayout;
        this.ItemTemplate = itemTemplate;
    }
}
