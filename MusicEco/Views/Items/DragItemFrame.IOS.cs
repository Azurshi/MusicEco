#if IOS
using MusicEco.Views.Buttons;
using System.Windows.Input;

namespace MusicEco.Views.Items;

public partial class DragItemFrame {
    private async Task PlatformDrag(MoveButton moveButton, ICommand command, Microsoft.Maui.Controls.DragStartingEventArgs e) {
        var position = e.GetPosition(moveButton);
        if (position is not Point point
            || point.X < 0
            || point.Y < 0
            || point.X > moveButton.Width
            || point.Y > moveButton.Height) {
            e.Cancel = true;
            return;
        }
        command.Execute(this.BindingContext);
        this.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(10), this.Reset);
    }
}
#endif
