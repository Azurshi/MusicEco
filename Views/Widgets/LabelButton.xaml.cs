namespace MusicEco.Views.Widgets;

public partial class LabelButton : ContentView {
    #region Binding
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(LabelButton),
            propertyChanged: OnTextChanged
        );
    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue) {
        ((LabelButton)bindable).InnerLabel.Text = (string)newValue;
    }
    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty TextAlignProperty =
    BindableProperty.Create(
        nameof(TextAlign),
        typeof(TextAlignment),
        typeof(LabelButton),
        propertyChanged: OnTextAlignChanged
    );
    private static void OnTextAlignChanged(BindableObject bindable, object oldValue, object newValue) {
        ((LabelButton)bindable).InnerLabel.HorizontalTextAlignment = (TextAlignment)newValue;
    }
    public string TextAlign {
        get => (string)GetValue(TextAlignProperty);
        set => SetValue(TextAlignProperty, value);
    }

    public static readonly BindableProperty SelectedProperty =
        BindableProperty.Create(
            nameof(Selected),
            typeof(bool),
            typeof(LabelButton),
            propertyChanged: OnSelectedChanged
        );
    private static void OnSelectedChanged(BindableObject bindable, object oldValue, object newValue) {
        bool isSelected = (bool)newValue;
        if (isSelected) {
            ((LabelButton)bindable).InnerLabel.FontAttributes = FontAttributes.Italic | FontAttributes.Bold;
        }
        else {
            ((LabelButton)bindable).InnerLabel.FontAttributes = FontAttributes.None;
        }
    }
    public string Selected {
        get => (string)GetValue(SelectedProperty);
        set => SetValue(SelectedProperty, value);
    }
    #endregion
    public event EventHandler<TappedEventArgs>? Clicked;
    public event EventHandler? Pressed;
    public event EventHandler? Released;
    public event EventHandler? Entered;
    public event EventHandler? Exited;
    public LabelButton() {
        InitializeComponent();
    }
    private void OnPressed(object sender, EventArgs e) {
        Pressed?.Invoke(((Label)sender).Parent, e);
    }
    private void OnReleased(object sender, EventArgs e) {
        Released?.Invoke(((Label)sender).Parent, e);
    }
    private void OnEntered(object sender, EventArgs e) {
        Entered?.Invoke(((Label)sender).Parent, e);
    }
    private void OnExited(object sender, EventArgs e) {
        Exited?.Invoke(((Label)sender).Parent, e);
    }
    private void OnClicked(object sender, TappedEventArgs e) {
        Clicked?.Invoke(((Label)sender).Parent, e);
    }
}