using Microsoft.Maui.Platform;
using MusicEco.SourceGeneration;
using MusicEco.Views.Buttons;
using System.Windows.Input;
#if WINDOWS
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
#endif
namespace MusicEco.Views.Items;

public partial class DragItemFrame: ItemFrame {
    private static readonly Type ThisType = typeof(DragItemFrame);
    [BindableAutoGen]
    public static readonly BindableProperty DragStartCommandProperty
        = Utility.Create<ICommand>(ThisType, null);
    [BindableAutoGen] 
    public static readonly BindableProperty DragOverCommandProperty
        = Utility.Create<ICommand?>(ThisType, null);
    [BindableAutoGen]
    public static readonly BindableProperty DropCommandProperty
        = Utility.Create<ICommand?>(ThisType, null);
    public DragItemFrame() {
        InitializeComponent();
        this.Loaded += this.DragItemFrame_Loaded;
    }
#if ANDROID || WINDOWS
    private MoveButton? _attachedMoveButton;
#endif
    private void DragItemFrame_Loaded(object? sender, EventArgs e) {
#if ANDROID || WINDOWS
        var moveButton = this.GetDragHandler();
        if (moveButton == null
            || ReferenceEquals(moveButton, this._attachedMoveButton)) {
            return;
        }
        this.GestureRecognizers.Remove(this.DragGR);
        this._attachedMoveButton?.GestureRecognizers.Remove(this.DragGR);

        moveButton.GestureRecognizers.Add(this.DragGR);
        this._attachedMoveButton = moveButton;
#endif
    }

    protected override void OnBindingContextChanged() {
        base.OnBindingContextChanged();
        var value = this.BindingContext;
        this.DropGR.DragOverCommandParameter = value;
        this.DropGR.DropCommandParameter = value;
    }
    private MoveButton? GetDragHandler() {
        return this.GetVisualTreeDescendants().OfType<MoveButton>().FirstOrDefault();
    }
    private async void DragGR_DragStarting(object sender, Microsoft.Maui.Controls.DragStartingEventArgs e) {
        var command = this.DragStartCommand;
        if (command == null || !command.CanExecute(this.BindingContext)) {
            e.Cancel = true;
        }
        else {
            var moveButton = this.GetDragHandler();
            if (moveButton == null) {
                e.Cancel = true;
                return;
            }
            await this.PlatformDrag(moveButton, command, e);
        }
    }

    private void DropGR_Drop(object? sender, DropEventArgs e) {
        this.Container.BackgroundColor = Colors.Transparent;
    }

    private void DropGR_DragOver(object? sender, Microsoft.Maui.Controls.DragEventArgs e) {
        this.Container.BackgroundColor = DynamicColors.ButtonHighlightColor;
    }

    private void DropGR_DragLeave(object? sender, Microsoft.Maui.Controls.DragEventArgs e) {
        this.Container.BackgroundColor = Colors.Transparent;
    }
    public void Reset() {
        this.Container.Opacity = 1.0;
    }

    private void DragGR_DropCompleted(object sender, Microsoft.Maui.Controls.DropCompletedEventArgs e) {
        this.Reset();
    }
}