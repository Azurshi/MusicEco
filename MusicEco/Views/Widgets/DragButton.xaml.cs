using MusicEco.ViewModels;

namespace MusicEco.Views.Widgets;
public partial class DragButton : Label {
    public DragButton() {
        InitializeComponent();
        PreviousColor = BackgroundColor;
    }
    private static readonly Color HoverColor = (Color)Application.Current!.Resources["HoverColor"];
    private Color PreviousColor;
    private void OnEntered(object sender, EventArgs e) {
        PreviousColor = BackgroundColor;
        BackgroundColor = HoverColor;
        ((IDraggable)this.BindingContext).IsDraggable = true;
    }
    private void OnExited(object sender, EventArgs e) {
        BackgroundColor = PreviousColor;
        ((IDraggable)this.BindingContext).IsDraggable = false;
    }
}