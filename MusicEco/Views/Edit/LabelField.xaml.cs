using MusicEco.ViewModels;
using System.Diagnostics;

namespace MusicEco.Views.Edit;

public partial class LabelField : Border, IEditableView {
    #region Binding
    private static readonly Type ThisType = typeof(LabelField);
    public static readonly BindableProperty TextProperty
        = Utility.Create<string>(ThisType, bindingMode: BindingMode.TwoWay,
            propertyChanged: (obj, _, value) => {
                ((LabelField)obj).InnerLabel.Text = (string)value;
            });
    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public static readonly BindableProperty KeyboardProperty
        = Utility.Create<Microsoft.Maui.Keyboard>(ThisType,
            (obj, _, value) => {
                ((LabelField)obj).EditField.Keyboard = (Microsoft.Maui.Keyboard)value;
            });
    public Microsoft.Maui.Keyboard Keyboard {
        get => (Microsoft.Maui.Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }
    #endregion
    public LabelField() {
        InitializeComponent();
    }
    #region IEdit
    public static BindableProperty ModeProperty = IEditableView.CreateBinding<LabelField>();
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
}