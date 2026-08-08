using MusicEco.Views.Overlays;
using System.Numerics;

namespace MusicEco.Views.Shell;

public partial class AppOverlay: ContentView {
    private Vector2? _fixedPosition = null;
    private Vector2? _dynamicSize = null;
    public AppOverlay() {
        InitializeComponent();
    }

    private void Grid_Tapped(object sender, TappedEventArgs e) {
        OnOverlayClosing(null, EventArgs.Empty);
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
            } else {
                throw new InvalidOperationException();
            }
        }
        else if (this._fixedPosition != null) {
            if (this.FixedContainer.Content is IOverlay overlay) {
                overlay.ForceClose();
            } else {
                throw new InvalidOperationException();
            }
        }
    }
    private void DeleteButton_Clicked(object sender, EventArgs e) {
        ForceCloseOverlay();
    }

    private void OnOverlayClosing(object? sender, EventArgs e) {
        this.IsVisible = false;
        this.DynamicContainer.Content = null;
        this.FixedContainer.Content = null;
    }
}

public partial class AppOverlay {
    public void ShowDynamic(Vector2 size, IOverlay overlay) {
        overlay.Closing += this.OnOverlayClosing;
        this._dynamicSize = size;
        if (overlay is View overlayView) {
            this.DynamicContainer.Content = overlayView;
        } else {
#pragma warning disable CA2208
            throw new ArgumentException(nameof(overlay));
#pragma warning restore CA2208
        }
        this.IsVisible = true;
        this.DynamicOverlay.IsVisible = true;
        this.FixedOverlay.IsVisible = false;
        this.UpdateDynamicSize();
    }
    public void ShowFixed(Vector2 position, IOverlay overlay) {
        overlay.Closing += this.OnOverlayClosing;
        this._fixedPosition = position;
        if (overlay is View overlayView) {
            this.FixedContainer.Content = overlayView;
        }
        else {
#pragma warning disable CA2208
            throw new ArgumentException(nameof(overlay));
#pragma warning restore CA2208
        }
        Rect layoutBound = new(
            position.X, position.Y,
            AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize);
        this.IsVisible = true;
        this.DynamicOverlay.IsVisible = false;
        this.FixedOverlay.IsVisible = true;
        AbsoluteLayout.SetLayoutBounds(this.FixedViewContainer, layoutBound);
    }
}

