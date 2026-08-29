namespace MusicEco.Views.Buttons;

public partial class MoveButton: Label {
    public MoveButton() {
        InitializeComponent();
        this.PlatformInitialize();
    }
#if WINDOWS
    private void PlatformInitialize() {
        // Not needed on Android
        var pointerGestureRecognizer = new PointerGestureRecognizer();
        pointerGestureRecognizer.PointerEntered += this.PointerGestureRecognizer_PointerEntered;
        pointerGestureRecognizer.PointerExited += this.PointerGestureRecognizer_PointerExited;
        this.GestureRecognizers.Add(pointerGestureRecognizer);
    }
    private void MoveButton_Loaded(object? sender, EventArgs e) {
        throw new NotImplementedException();
    }

    private void PointerGestureRecognizer_PointerEntered(object? sender, PointerEventArgs e) {
        this.BackgroundColor = DynamicColors.ButtonHighlightColor;
    }

    private void PointerGestureRecognizer_PointerExited(object? sender, PointerEventArgs e) {
        this.BackgroundColor = Colors.Transparent;
    }
#else
    private void PlatformInitialize() {
    }
#endif
}