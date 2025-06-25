using MusicEco.ViewModels;

namespace MusicEco.Views.PageExtensions;
public interface IDragSupportPage {
    public abstract void DropGestureRecognizer_Drop(object sender, DropEventArgs e);
    public abstract void DragGestureRecognizer_DragStarting(object sender, DragStartingEventArgs e);
}
public class DragExtension {
    public void Drop(object sender, DropEventArgs e) {
        e.Handled = true;
    }
    public void DragStarting(object sender, DragStartingEventArgs e) {
        if (sender is GestureRecognizer recognizer) {
            if (!((IDraggable)recognizer.BindingContext).IsDraggable) {
                e.Cancel = true;
                ((IDraggable)recognizer.BindingContext).IsDragged = false;
            }
        }
    }
}
