using MusicEco.Views.Overlays;
using System.Diagnostics;
using System.Numerics;

namespace MusicEco.Views.Shell;

public partial class AppOverlay: ContentView, IOverlayService {
    private Vector2? _fixedPosition = null;
    private Vector2? _dynamicSize = null;

    public AppOverlay() {
        InitializeComponent();
        this.SizeChanged += this.AppOverlay_SizeChanged;
        this.FixedOverlay.IsVisible = false;
        this.DynamicOverlay.IsVisible = false;
    }

    private void AppOverlay_SizeChanged(object? sender, EventArgs e) {
        if (this.FixedContainer.Content is IOverlay overlay) {
            overlay.ForceClose();
        }
        else if (this.DynamicContainer.Content is IOverlay) {
            this.UpdateDynamicSize();
        }
    }

    private void Grid_Tapped(object sender, TappedEventArgs e) {
        this.ForceCloseOverlay();
    }
    private void UpdateDynamicSize() {
        if (this._dynamicSize != null) {
            var size = this._dynamicSize.Value;
            this.DynamicContainer.WidthRequest = size.X * this.Width;
            this.DynamicContainer.HeightRequest = size.Y * this.Height;
        }
    }
    private void Grid_SizeChanged(object sender, EventArgs e) {
        if (this._dynamicSize != null) {
            UpdateDynamicSize();
        }
        else if (this._fixedPosition != null) {
            ForceCloseOverlay();
        }
    }
    private void ForceCloseOverlay() {
        if (this._dynamicSize != null) {
            if (this.DynamicContainer.Content is IOverlay overlay) {
                overlay.ForceClose();
            }
            else {
                // Seem like we need to ignore this since two overlay can exists now or because of race condition
                //throw new InvalidOperationException();
            }
        }
        if (this._fixedPosition != null) {
            if (this.FixedContainer.Content is IOverlay overlay) {
                overlay.ForceClose();
            }
            else {
                //throw new InvalidOperationException();
            }
        }
    }
    private void DeleteButton_Clicked(object sender, EventArgs e) {
        ForceCloseOverlay();
    }

    private void OnOverlayClosed(object? sender, EventArgs e) {
        this.IsVisible = false;
        this.FixedOverlay.IsVisible = false;
        this.DynamicOverlay.IsVisible = false;
        this.DynamicContainer.Content = null;
        this.FixedContainer.Content = null;
        Debug.WriteLine("Overlay closed");
    }
}

public partial class AppOverlay {
    public void ShowDynamic(Vector2 size, IOverlay overlay) {
        overlay.Closed += this.OnOverlayClosed;
        this._dynamicSize = size;
        if (overlay is View overlayView) {
            this.DynamicContainer.Content = overlayView;
        }
        else {
#pragma warning disable CA2208
            throw new ArgumentException(nameof(overlay));
#pragma warning restore CA2208
        }
        this.IsVisible = true;
        this.DynamicOverlay.IsVisible = true;
        //this.FixedOverlay.IsVisible = false;
        //this.FixedContainer.Content = null;
        this.UpdateDynamicSize();
        Debug.WriteLine("Overlay dynamic");
    }
    public void ShowFixed(Vector2 position, IOverlay overlay) {
        overlay.Closed += this.OnOverlayClosed;
        this._fixedPosition = position;
        if (overlay is View overlayView) {
            this.FixedContainer.Content = overlayView;
        }
        else {
#pragma warning disable CA2208
            throw new ArgumentException(nameof(overlay));
#pragma warning restore CA2208
        }
        Vector2 size = new(0, 0);
        if (overlay is View view) {
            size.X = (float)view.Width;
            size.Y = (float)view.Height;
            view.SizeChanged += (s, e) => {
                Vector2 bound = new((float)this.Width, (float)this.Height);
                Rect layoutBound = new(
                    position.X, position.Y,
                    AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize);
                if (position.X + view.Width > bound.X) {
                    layoutBound.X = position.X - view.Width;
                }
                if (position.Y + view.Height > bound.Y) {
                    layoutBound.Y = position.Y - view.Height;
                }
                AbsoluteLayout.SetLayoutBounds(this.FixedViewContainer, layoutBound);
            };
        }
        else {
            Rect layoutBound = new(
                position.X, position.Y,
                AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize);
            AbsoluteLayout.SetLayoutBounds(this.FixedViewContainer, layoutBound);
        }
        this.IsVisible = true;
        //this.DynamicOverlay.IsVisible = false;
        this.FixedOverlay.IsVisible = true;
        //this.DynamicContainer.Content = null;
        Debug.WriteLine("Overlay fixed");
    }
}