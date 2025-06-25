using Domain;
using System.Diagnostics;

namespace MusicEco.Views.Components;

public partial class Overlay : ContentView
{
    private static Overlay? _current;
    public static Overlay? Current => _current;
    private bool autoMove = false;
    public Overlay()
	{
		InitializeComponent();
	}
    public void SetAsCurrent() {
        _current = this;
    }
    public void Stop() {
        this.IsVisible = false;
        this.autoMove = false;
        Presenter.Content = null;
    }
    #region StartMethods
    private static readonly Vector2 _defaultSize = new(-1, -1);
    public void Start(View view, Vector2 position, Vector2? size = null, bool autoMove = false) {
        this.IsVisible = true;
        Presenter.Content = view;
        Vector2 size_ = size ?? _defaultSize;
        AbsoluteLayout.SetLayoutBounds(Presenter, new Rect(
            position.X, position.Y,
            size_.X, size_.Y
            ));
        this.autoMove = autoMove;
    }
    public void StartAuto(View view) {
        Vector2 pad = new(200, 50);
        this.IsVisible = true;
        Presenter.Content = view;
        AbsoluteLayout.SetLayoutBounds(Presenter, new Rect(
            pad.X, pad.Y,
            this.Width - pad.X * 2, this.Height - pad.Y * 2));
    }
    public void StartAbsolute(View view) {
        this.IsVisible = true;
        Presenter.Content = view;
        AbsoluteLayout.SetLayoutBounds(Presenter, new Rect(
            0, 0, this.Width, this.Height));
    }
    #endregion
    #region Signals
    public void MoveContent(float? x, float? y) {
        Rect rect = AbsoluteLayout.GetLayoutBounds(Presenter);
        if (x != null && y != null) {
            AbsoluteLayout.SetLayoutBounds(Presenter, new Rect(
                (double)x, (double)y, rect.Width, rect.Height
                ));
        }
        else if (x != null) {
            AbsoluteLayout.SetLayoutBounds(Presenter, new Rect(
                (double)x, rect.Y, rect.Width, rect.Height
                ));
        }
        else if (y != null) {
            AbsoluteLayout.SetLayoutBounds(Presenter, new Rect(
                rect.X, (double)y, rect.Width, rect.Height
                ));
        }
    }
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) {
        this.IsVisible = false;
    }

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e) {
        Debug.WriteLine(autoMove);
        if (autoMove) {
            Debug.WriteLine("Moving");
            MoveContent(null, (float)e.TotalY);
            Debug.WriteLine($"X {e.TotalX} Y {e.TotalY}");

        }
    }

    private void PointerGestureRecognizer_PointerMoved(object sender, PointerEventArgs e) {
        Debug.WriteLine(autoMove);
        if (autoMove) {
            Debug.WriteLine("Moving");
            Point? point = e.GetPosition(this);
            if (point != null) {
                MoveContent(null, (float)point.Value.Y);
                Debug.WriteLine($"X {point.Value.X} Y {point.Value.Y}");
            }
        }
    }
    #endregion
}