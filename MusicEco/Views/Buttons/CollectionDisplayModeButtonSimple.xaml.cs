using MusicEco.SourceGeneration;
using MusicEco.ViewModels;

namespace MusicEco.Views.Buttons;

public partial class CollectionDisplayModeButtonSimple: ContentView {
    private static readonly Type ThisType = typeof(CollectionDisplayModeButtonSimple);
    [BindedProperty]
    public partial double SizeRequest { get; set; }
    public static readonly BindableProperty SizeRequestProperty
        = Utility.Create<double>(ThisType, 64.0);
    [BindableAutoGen]
    public static readonly BindableProperty DisplayModeProperty
        = Utility.Create<CollectionDisplayMode>(ThisType, CollectionDisplayMode.SimpleList,
            propertyChanged: (b, _, v) => {
                var This = (CollectionDisplayModeButtonSimple)b;
                var displayMode = (CollectionDisplayMode)v;
                This.SetDisplayMode(displayMode);
            },
            bindingMode: BindingMode.TwoWay);
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
            this.GridButton.ChangeToActiveState();
            this.ListButton.ChangeToInactiveState();
        }
        else {
            this.GridButton.ChangeToInactiveState();
            this.ListButton.ChangeToActiveState();
        }
        DisplayModeChanged?.Invoke(this, mode);
    }

    private void GridButton_Tapped(object sender, TappedEventArgs e) {
        this.DisplayMode = CollectionDisplayMode.SimpleGrid;

    }
    private void ListButton_Tapped(object sender, TappedEventArgs e) {
        this.DisplayMode = CollectionDisplayMode.SimpleList;

    }
    private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e) {
        if (sender is View view) {
            view.BackgroundColor = DynamicColors.ButtonHighlightColor;
        }
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e) {
        if (sender is View view) {
            view.BackgroundColor = Colors.Transparent;
        }
    }

}