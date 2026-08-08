using MusicEco.ViewModels;

namespace MusicEco.Views.Buttons;

public partial class CollectionDisplayModeButtonSimple: ContentView {
    private static readonly Type ThisType = typeof(CollectionDisplayModeButtonSimple);
    public static readonly BindableProperty DisplayModeProperty
        = Utility.Create<CollectionDisplayMode>(ThisType, CollectionDisplayMode.SimpleList,
            propertyChanged: (b, _, v) => {
                var This = (CollectionDisplayModeButtonSimple)b;
                var displayMode = (CollectionDisplayMode)v;
                This.SetDisplayMode(displayMode);
            },
            bindingMode: BindingMode.TwoWay);
    public CollectionDisplayMode DisplayMode {
        get => (CollectionDisplayMode)GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }
    public event EventHandler<CollectionDisplayMode>? DisplayModeChanged;
    public CollectionDisplayModeButtonSimple() {
        InitializeComponent();
        this.SetDisplayMode(this.DisplayMode);
    }
    private void SetDisplayMode(CollectionDisplayMode mode) {
        if (mode != CollectionDisplayMode.SimpleGrid
            && mode != CollectionDisplayMode.SimpleList) {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }
        if (mode == CollectionDisplayMode.SimpleGrid) {
            this.GridButton.InputTransparent = true;
            this.ListButton.InputTransparent = false;
            this.GridButton.Opacity = 1.0;
            this.ListButton.Opacity = 0.5;
        }
        else {
            this.GridButton.InputTransparent = false;
            this.ListButton.InputTransparent = true;
            this.GridButton.Opacity = 0.5;
            this.ListButton.Opacity = 1.0;
        }
        DisplayModeChanged?.Invoke(this, mode);
    }

    private void GridButton_Clicked(object sender, EventArgs e) {
        this.DisplayMode = CollectionDisplayMode.SimpleGrid;
    }

    private void ListButton_Clicked(object sender, EventArgs e) {
        this.DisplayMode = CollectionDisplayMode.SimpleList;
    }
}