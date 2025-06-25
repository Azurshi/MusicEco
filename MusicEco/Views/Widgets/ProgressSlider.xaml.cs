namespace MusicEco.Views.Widgets;

public partial class ProgressSlider : ContentView {
    #region Binding
    private static readonly Type ThisType = typeof(ProgressSlider);
    public static readonly BindableProperty PercentProperty =
        Utility.Create<float>(ThisType, bindingMode: BindingMode.TwoWay, propertyChanged:
            (b, _, v) => {
                ProgressSlider This = (ProgressSlider)b;
                if (!This._isDragging) {
                    This.SetProgress((float)v);
                }
            }
    );
    public float Percent {
        get => (float)GetValue(PercentProperty);
        set {
            SetProgress(value);
            SetValue(PercentProperty, value);
        }
    }
    public static readonly BindableProperty IconRadiusProperty =
        Utility.Create<float>(ThisType);
    public float IconRadius {
        get => (float)GetValue(IconRadiusProperty);
        set => SetValue(IconRadiusProperty, value);
    }
    public static readonly BindableProperty OrientationProperty =
        Utility.Create<StackOrientation>(ThisType);
    public StackOrientation Orientation {
        get => (StackOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    public static readonly BindableProperty InvertedProperty =
        Utility.Create<bool>(ThisType);
    public bool Inverted {
        get => (bool)GetValue(InvertedProperty);
        set => SetValue(InvertedProperty, value);
    }
    #endregion
    public bool IsDragging => _isDragging;
    private bool _isDragging = false;
    private double _lastPercent = 0;
    private double _lastDraggedPercent = 0;
    public ProgressSlider() {
        InitializeComponent();
        IconRadius = 10;
        Orientation = StackOrientation.Horizontal;
        Inverted = false;
        Dispatcher.Dispatch(()=>SetProgress(Percent));
    }
    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e) {
        if (e.StatusType == GestureStatus.Started) {
            _isDragging = true;
            _lastPercent = Percent;
        }
        else if (e.StatusType == GestureStatus.Running) {
            float sign = 1;
            if (Inverted) {
                sign = -1;
            }
            if (Orientation == StackOrientation.Horizontal) {
                double percent = _lastPercent + e.TotalX * sign / HolderLayout.Width;
                percent = Math.Clamp(percent, 0, 1);
                _lastDraggedPercent = percent;
                SetProgress((float)percent);
            }
            else {
                double percent = _lastPercent + e.TotalY * sign / HolderLayout.Height;
                percent = Math.Clamp(percent, 0, 1);
                _lastDraggedPercent = percent;
                SetProgress((float)percent);
            }
        }
        else if (e.StatusType == GestureStatus.Completed) {
            _isDragging = false;
            Percent = (float)_lastDraggedPercent;
        }

    }
    private void SetProgress(float percent) {
        AbsoluteLayout.SetLayoutBounds(UnderLabel, new Rect(0, 0, HolderLayout.Width, HolderLayout.Height));
        if (Orientation == StackOrientation.Horizontal) {
            double width = HolderLayout.Width * percent;
            AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, width, HolderLayout.Height));
            double x = width - IconRadius;
            double y = -IconRadius + HolderLayout.Height / 2;
            AbsoluteLayout.SetLayoutBounds(IconImage, new Rect(x, y, IconRadius * 2, IconRadius * 2));
        }
        else {
            double height = HolderLayout.Height * percent;
            AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, HolderLayout.Width, height));
            double x = -IconRadius + HolderLayout.Width / 2;
            double y = height - IconRadius;
            AbsoluteLayout.SetLayoutBounds(IconImage, new Rect(x, y, IconRadius * 2, IconRadius * 2));
        }
    }
}