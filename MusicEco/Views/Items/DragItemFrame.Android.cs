#if ANDROID
using MusicEco.Views.Buttons;
using System.Windows.Input;

namespace MusicEco.Views.Items;

internal sealed class AnchoredDragShadowBuilder: Android.Views.View.DragShadowBuilder {
    private readonly Android.Views.View _view;
    private readonly int _anchorX;
    private readonly int _anchorY;
    public AnchoredDragShadowBuilder(Android.Views.View view, Android.Views.MotionEvent motionEvent): base(view) {
        this._view = view;
        var location = new int[2];
        view.GetLocationOnScreen(location);
        this._anchorX = Math.Clamp(
            (int)Math.Round(motionEvent.RawX) - location[0],
            1,
            Math.Max(0, view.Width - 1));
        this._anchorY = Math.Clamp(
            (int)Math.Round(motionEvent.RawY) - location[1],
            1,
            Math.Max(0, view.Height - 1));
    }
    public override void OnProvideShadowMetrics(Android.Graphics.Point? outShadowSize, Android.Graphics.Point? outShadowTouchPoint) {
        base.OnProvideShadowMetrics(outShadowSize, outShadowTouchPoint);
        outShadowSize?.Set(this._view.Width, this._view.Height);
        outShadowTouchPoint?.Set(this._anchorX, this._anchorY);
    }
}
public partial class DragItemFrame {
    private async Task PlatformDrag(MoveButton moveButton, ICommand command, Microsoft.Maui.Controls.DragStartingEventArgs e) {
        var platformArgs = e.PlatformArgs;
        if (this.Handler?.PlatformView is Android.Views.View nativeView
            && platformArgs?.MotionEvent is Android.Views.MotionEvent motionEvent) {
            platformArgs.SetDragShadowBuilder(new AnchoredDragShadowBuilder(nativeView, motionEvent));
            if (OperatingSystem.IsAndroidVersionAtLeast(24)) {
                platformArgs.SetDragFlags(
                    Android.Views.DragFlags.Global
                    | Android.Views.DragFlags.GlobalUriRead
                    | Android.Views.DragFlags.Opaque);
            }
        }
        command.Execute(this.BindingContext);
        this.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), this.Reset);
    }
}
#endif