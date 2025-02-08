namespace MusicEco.Views.Widgets;

using MusicEco.Common.Events;
using System.Diagnostics;
#if ANDROID
using Android.Views;
#endif
public partial class MoveLabel : ContentView {
    public event EventHandler? SlotPressed;
    public event EventHandler? PanStarted;
    public event EventHandler<Vector2EventArgs>? PanPositionChanged;
    public event EventHandler? PanCompleted;
    public MoveLabel() {
        InitializeComponent();
    }
    private void OnPressed(object sender, PointerEventArgs e) {
        Point point = e.GetPosition(this.Content) ?? new Point(0, 0);
        initialX = point.X;
        initialY = point.Y;
    }
    private double initialX = 0;
    private double initialY = 0;
    private void OnPanUpdated(object sender, PanUpdatedEventArgs e) {
#if ANDROID
        var motionEvent = e.GetType().GetProperty("NativeMotionEvent")?.GetValue(e) as MotionEvent;
        if (motionEvent != null) {
            initialX = motionEvent.GetX();
            initialY = motionEvent.GetY();
        }
#endif
        if (e.StatusType == GestureStatus.Started) {
            PanStarted?.Invoke(((Label)sender).Parent, e);
            SlotPressed?.Invoke(this.Content, EventArgs.Empty);
        }
        else if (e.StatusType == GestureStatus.Running) {
            double x = e.TotalX;
            double y = e.TotalY;
            PanPositionChanged?.Invoke(this.Content, new(initialX + x, initialY + y));
        }
        else if (e.StatusType == GestureStatus.Completed) {
            Debug.WriteLine("Completed");
            initialX = 0;
            initialY = 0;
            PanCompleted?.Invoke(this.Content, EventArgs.Empty);
        }
    }
    private static MoveLabel Cast(object obj) {
        return (MoveLabel)((Label)((GestureRecognizer)obj).Parent).Parent;
    }
}