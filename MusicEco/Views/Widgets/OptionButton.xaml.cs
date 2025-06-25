namespace MusicEco.Views.Widgets;
public partial class OptionButton : Border {
    public event EventHandler<TappedEventArgs>? Clicked;
    public OptionButton() {
        InitializeComponent();
        PreviousColor = InnerLabel.BackgroundColor;
    }
    private static readonly Color HoverColor = (Color)Application.Current!.Resources["HoverColor"];
    private Color PreviousColor;
    private void OnEntered(object sender, EventArgs e) {
        PreviousColor = this.InnerLabel.BackgroundColor;
        InnerLabel.BackgroundColor = HoverColor;
    }
    private void OnExited(object sender, EventArgs e) {
        InnerLabel.BackgroundColor = PreviousColor;
    }
    private void OnClicked(object sender, TappedEventArgs e) {
        InnerLabel.BackgroundColor = PreviousColor;
        Clicked?.Invoke(this, e);
    }
}