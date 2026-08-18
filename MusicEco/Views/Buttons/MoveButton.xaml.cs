using MusicEco.SourceGeneration;

namespace MusicEco.Views.Buttons;

public partial class MoveButton: Label {
    private static readonly Type ThisType = typeof(MoveButton);
    [BindableAutoGen]
    public static readonly BindableProperty IsDraggableProperty
        = Utility.Create<bool>(ThisType, false, bindingMode: BindingMode.TwoWay);
    public MoveButton() {
        InitializeComponent();
    }

    private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e) {
        this.BackgroundColor = Colors.Yellow;
        this.IsDraggable = true;
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e) {
        this.BackgroundColor = Colors.Transparent;
        this.IsDraggable = false;
    }
}