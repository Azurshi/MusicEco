namespace MusicEco.Views.Widgets;

public partial class ImagePro : ContentView
{
    public event EventHandler? Clicked;
    public event EventHandler? Pressed;
    public event EventHandler? Released;
    public Thickness InnerPadding {
        get {
            return InnerPadding;
        }
        set {
            InnerPadding = value;
        }
    }
    public Thickness InnerMargin {
        get {
            return InnerImage.Margin;
        }
        set {
            InnerImage.Margin = value;
        }
    }
    public ImageSource Source {
        get {
            return InnerImage.Source;
        }
        set {
            InnerImage.Source = value;
        }
    }

    public Aspect Aspect {
        get {
            return InnerImage.Aspect;
        }
        set {
            InnerImage.Aspect = value;
        }
    }
    public double ImageRotation {
        get {
            return InnerImage.Rotation;
        }
        set {
            InnerImage.Rotation = value;
        }
    }
    public ImagePro() {
        InitializeComponent();
    }
    private void OnPressed(object sender, EventArgs e) {
        InnerImage.Scale = 0.9;
        InnerImage.BackgroundColor = Colors.Transparent;
        Pressed?.Invoke(this.Content, e);
    }
    private void OnReleased(object sender, EventArgs e) {
        InnerImage.Scale = 1;
        InnerImage.BackgroundColor = Colors.LightBlue;
        Released?.Invoke(this.Content, e);
    }
    private void OnEntered(object sender, EventArgs e) {
        InnerImage.BackgroundColor = Colors.LightBlue;
    }
    private void OnExited(object sender, EventArgs e) {
        InnerImage.BackgroundColor = Colors.Transparent;
    }
    private void OnClicked(object sender, EventArgs e) {
        Clicked?.Invoke(this.Content, e);
    }
}