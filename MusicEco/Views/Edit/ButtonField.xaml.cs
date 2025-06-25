using MusicEco.ViewModels;
using System.Diagnostics;
using System.Windows.Input;

namespace MusicEco.Views.Edit;

public partial class ButtonField : Border, IEditableView {
    #region Binding
    private static readonly Type ThisType = typeof(ButtonField);
    public static readonly BindableProperty TextProperty
        = Utility.Create<string>(ThisType, bindingMode: BindingMode.TwoWay,
            propertyChanged: (obj, _, value) => {
                ((ButtonField)obj).InnerLabel.Text = (string)value;
            });
    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public static readonly BindableProperty KeyboardProperty
        = Utility.Create<Microsoft.Maui.Keyboard>(ThisType,
            (obj, _, value) => {
                ((ButtonField)obj).EditField.Keyboard = (Microsoft.Maui.Keyboard)value;
            });
    public Microsoft.Maui.Keyboard Keyboard {
        get => (Microsoft.Maui.Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }
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
    public static readonly BindableProperty AvailableProperty = Utility.Create<bool>(ThisType,
        propertyChanged: (b, _, v) => {
            ButtonField This = (ButtonField)b;
            bool value = (bool)v;
            if (value) {
                This.InnerLabel.TextColor = This.PreviousTextColor;
            }
            else {
                This.InnerLabel.TextColor = DisabledColor;
            }
        },
        defaultValue: true
    ); // Not tested due to no usecase
    #endregion
    public ButtonField() {
        InitializeComponent();
        PreviousTextColor = this.InnerLabel.TextColor;
    }
    #region IEdit
    public static BindableProperty ModeProperty = IEditableView.CreateBinding<LabelField>();
    private static readonly Color DisabledColor = (Color)Application.Current!.Resources["DisabledColor"];
    private Color PreviousTextColor;
    public ViewMode Mode {
        get => (ViewMode)GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }
    void IEditableView.OnEdit() {
        InnerLabel.IsVisible = false;
        EditField.IsVisible = true;
        EditField.Text = InnerLabel.Text;
    }
    void IEditableView.OnConfirm() {
        InnerLabel.IsVisible = true;
        EditField.IsVisible = false;
        if (Keyboard == Microsoft.Maui.Keyboard.Numeric) {
            string? value = Validator.Float(EditField.Text);
            if (value != null) {
                Text = value;
            }
        }
        else {
            Text = Validator.Name(EditField.Text);
        }
    }
    void IEditableView.OnCancel() {
        InnerLabel.IsVisible = true;
        EditField.IsVisible = false;
        EditField.Text = InnerLabel.Text;
    }
    #endregion
    #region Signals
    public event EventHandler<TappedEventArgs>? Clicked;
    private void OnLabelClicked(object sender, TappedEventArgs e) {
        Clicked?.Invoke(this, e);
        Command?.Execute(CommandParameter);
    }
    #endregion
}