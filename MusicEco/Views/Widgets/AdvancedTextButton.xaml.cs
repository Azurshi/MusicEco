using System.Diagnostics;
using System.Windows.Input;

namespace MusicEco.Views.Widgets;

public partial class AdvancedTextButton : Label
{
    #region Binding
    private static readonly Type ThisType = typeof(AdvancedTextButton);
    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly BindableProperty CommandProperty =
        Utility.Create<ICommand>(ThisType);
    public object? CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    public static readonly BindableProperty CommandParameterProperty =
        Utility.Create<object>(ThisType);
    public bool IsClickable {
        get => (bool)GetValue(IsClickableProperty);
        set => SetValue(IsClickableProperty, value);
    }
    public static readonly BindableProperty IsClickableProperty =
        Utility.Create<bool>(ThisType, (s, _, v) => {
            ((AdvancedTextButton)s).UpdateClickable();
        });
    #endregion
    public event EventHandler<TappedEventArgs>? Clicked;
    public AdvancedTextButton() {
        InitializeComponent();
        PreviousColor = this.BackgroundColor;
        PreviousTextColor = this.TextColor;
        this.UpdateClickable();
    }
    #region Signal
    private static readonly Color HoverColor = (Color)Application.Current!.Resources["HoverColor"];
    private static readonly Color DisabledTextColor = (Color)Application.Current!.Resources["DisabledColor"];
    private Color PreviousColor;
    private Color PreviousTextColor;
    private void OnEntered(object sender, EventArgs e) {
        if (IsClickable) {
            PreviousColor = this.BackgroundColor;
            BackgroundColor = HoverColor;
        }
    }
    private void OnExited(object sender, EventArgs e) {
        if (IsClickable) {
            BackgroundColor = PreviousColor;
        }
    }
    private void OnClicked(object sender, TappedEventArgs e) {
        if (IsClickable) {
            Clicked?.Invoke(this, e);
            Command?.Execute(CommandParameter);
        }
    }
    private void UpdateClickable() {
        if (IsClickable) {
            this.TextColor = this.PreviousTextColor;
        }
        else if (this.TextColor != DisabledTextColor) {
            this.PreviousColor = this.TextColor;
            this.TextColor = DisabledTextColor;
        }
    }
    #endregion
}