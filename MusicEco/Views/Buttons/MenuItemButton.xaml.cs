
using System.Windows.Input;

namespace MusicEco.Views.Buttons;

public partial class MenuItemButton: ContentView {
    private static readonly Type ThisType = typeof(MenuItemButton);
    public static readonly BindableProperty CommandProperty
        = Utility.Create<ICommand?>(ThisType,
            propertyChanged: (b, oldValue, newValue) => {
                var This = (MenuItemButton)b;
                var oldCommand = (ICommand?)oldValue;
                var newCommand = (ICommand?)newValue;
                oldCommand?.CanExecuteChanged -= This.NewCommand_CanExecuteChanged;
                newCommand?.CanExecuteChanged += This.NewCommand_CanExecuteChanged;
                This.RefreshCommandState();
            });
    public ICommand? Command {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly BindableProperty TextProperty
        = Utility.Create<string>(ThisType, string.Empty,
            propertyChanged: (b, _, v) => {
                var This = (MenuItemButton)b;
                var text = (string)v;
                This.InnerLabel.Text = text;
            });
    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public event EventHandler? Tapped;
    public MenuItemButton() {
        InitializeComponent();
    }

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e) {
        Tapped?.Invoke(this, e);
        Command?.Execute(this.BindingContext);
    }

    private void PointerGestureRecognizer_PointerEntered(object? sender, PointerEventArgs e) {
        this.BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void PointerGestureRecognizer_PointerExited(object? sender, PointerEventArgs e) {
        this.BackgroundColor = Colors.Transparent;
    }
    protected override void OnHandlerChanged() {
        base.OnHandlerChanged();
        this.Command?.CanExecuteChanged -= this.NewCommand_CanExecuteChanged;
    }
    private void NewCommand_CanExecuteChanged(object? sender, EventArgs e) {
        RefreshCommandState();
    }
    private void RefreshCommandState() {
        if (this.Command?.CanExecute(this.BindingContext) ?? false) {
            this.Opacity = 1;
        }
        else {
            this.Opacity = 0.4;
        }
    }
}