using MusicEco.ViewModels;

namespace MusicEco.Views.Widgets;

public partial class BackButton : Label {
    #region Binding
    private static readonly Type ThisType = typeof(BackButton);
    public string Route {
        get => (string)GetValue(RouteProperty);
        set => SetValue(RouteProperty, value);
    }
    public static readonly BindableProperty RouteProperty = Utility.Create<string>(ThisType);
    #endregion
    public event EventHandler<TappedEventArgs>? Clicked;
    public BackButton() {
        InitializeComponent();
        PreviousColor = this.BackgroundColor;
    }
    #region Signal
    private static readonly Color HoverColor = (Color)Application.Current!.Resources["HoverColor"];
    private Color PreviousColor;

    private async void OnClicked(object sender, TappedEventArgs e) {
        Clicked?.Invoke(this, e);
        await Navigator.GoToAsync(Route);
    }
    private void OnEntered(object sender, EventArgs e) {
        PreviousColor = this.BackgroundColor;
        BackgroundColor = HoverColor;
    }
    private void OnExited(object sender, EventArgs e) {
        BackgroundColor = PreviousColor;
    }
    #endregion
}