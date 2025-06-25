using System.Windows.Input;

namespace MusicEco.Views.Widgets;

public partial class ConfirmButton : Border
{
    #region Binding
    public ICommand ConfirmCommand {
        get => (ICommand)GetValue(ConfirmCommandProperty);
        set => SetValue(ConfirmCommandProperty, value);
    }
    public static readonly BindableProperty ConfirmCommandProperty =
    BindableProperty.Create(
        nameof(ConfirmCommand), typeof(ICommand), typeof(ConfirmButton),
        propertyChanged: (b, _, v) => {
            ((ConfirmButton)b).confirmCommand = (ICommand)v;
        });
    public object? ConfirmCommandParameter {
        get => GetValue(ConfirmCommandParameterProperty);
        set => SetValue(ConfirmCommandParameterProperty, value);
    }
    public static readonly BindableProperty ConfirmCommandParameterProperty =
        BindableProperty.Create(
        nameof(ConfirmCommandParameter), typeof(object), typeof(ConfirmButton),
        propertyChanged: (b, _, v) => {
            ((ConfirmButton)b).confirmCommandParameter = v;
        });
    public ICommand CancelCommand {
        get => (ICommand)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }
    public static readonly BindableProperty CancelCommandProperty =
    BindableProperty.Create(
        nameof(CancelCommand), typeof(ICommand), typeof(ConfirmButton),
        propertyChanged: (b, _, v) => {
            ((ConfirmButton)b).cancelCommand = (ICommand)v;
        });
    public object? CancelCommandParameter {
        get => GetValue(CancelCommandParameterProperty);
        set => SetValue(CancelCommandParameterProperty, value);
    }
    public static readonly BindableProperty CancelCommandParameterProperty =
        BindableProperty.Create(
        nameof(CancelCommandParameter), typeof(object), typeof(ConfirmButton),
        propertyChanged: (b, _, v) => {
            ((ConfirmButton)b).cancelCommandParameter = v;
        });
    #endregion
    private ICommand? confirmCommand;
    private object? confirmCommandParameter;
    private ICommand? cancelCommand;
    private object? cancelCommandParameter;
    public string ConfirmText {
        get => ConfirmLabel.Text;
        set => ConfirmLabel.Text = value;
    }
    public string CancelText {
        get => CancelLabel.Text;
        set => CancelLabel.Text = value;
    }
    public event EventHandler<TappedEventArgs>? ConfirmClicked;
    public event EventHandler<TappedEventArgs>? CancelClicked;
    public ConfirmButton()
	{
		InitializeComponent();
        PreviousConfirmColor = ConfirmLabel.BackgroundColor;
        PreviousCancelColor = CancelLabel.BackgroundColor;
	}
    private static readonly Color ConfirmHighlightColor = Colors.Green;
    private static readonly Color CancelHighlightColor = Colors.Red;
    private Color PreviousConfirmColor;
    private Color PreviousCancelColor;
    private void OnConfirm_Entered(object sender, PointerEventArgs e) {
        PreviousConfirmColor = ConfirmLabel.BackgroundColor;
        ConfirmLabel.BackgroundColor = ConfirmHighlightColor;
    }
    private void OnConfirm_Exited(object sender, PointerEventArgs e) {
        ConfirmLabel.BackgroundColor = PreviousConfirmColor;
    }
    private void OnCancel_Entered(object sender, PointerEventArgs e) {
        PreviousCancelColor = CancelLabel.BackgroundColor;
        CancelLabel.BackgroundColor = CancelHighlightColor;
    }
    private void OnCancel_Exited(object sender, PointerEventArgs e) {
        CancelLabel.BackgroundColor = PreviousCancelColor;
    }
    private void OnConfirm_Clicked(object sender, TappedEventArgs e) {
        ConfirmLabel.BackgroundColor = PreviousConfirmColor;
        ConfirmClicked?.Invoke(this, e);
        confirmCommand?.Execute(confirmCommandParameter);
    }
    private void OnCancel_Clicked(object sender, TappedEventArgs e) {
        CancelLabel.BackgroundColor = PreviousCancelColor;
        CancelClicked?.Invoke(this, e);
        cancelCommand?.Execute(cancelCommandParameter);
    }
}