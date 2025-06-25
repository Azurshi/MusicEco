namespace MusicEco.Views.Widgets;

public partial class ProgressBar : ContentView {
    #region Binding
    private static readonly Type ThisType = typeof(ProgressBar);
    public static readonly BindableProperty PercentProperty =
        Utility.Create<float>(ThisType, propertyChanged:
            (b, _, v) => ((ProgressBar)b).SetProgress((float)v)
    );
    public float Percent {
        get => (float)GetValue(PercentProperty);
        set {
            SetProgress(value);
            SetValue(PercentProperty, value);
        }
    }
    #endregion
    public ProgressBar() {
        InitializeComponent();
    }
    private void SetProgress(float percent) {
        AbsoluteLayout.SetLayoutBounds(UnderLabel, new Rect(0, 0, HolderLayout.Width, HolderLayout.Height));
        AbsoluteLayout.SetLayoutBounds(TextLabel, new Rect(0, 0, HolderLayout.Width, HolderLayout.Height));
        double width = HolderLayout.Width * percent;
        AbsoluteLayout.SetLayoutBounds(OverLabel, new Rect(0, 0, width, HolderLayout.Height));
        string text = $"{percent * 100:F2} %";
        TextLabel.Text = text;
    }
}