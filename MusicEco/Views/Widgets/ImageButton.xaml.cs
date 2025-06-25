using System.Diagnostics;
using System.Windows.Input;

namespace MusicEco.Views.Widgets;

public partial class ImageButton : Image
{
    #region Binding
    private static readonly Type ThisType = typeof(ImageButton);
    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly BindableProperty CommandProperty =
        Utility.Create<ICommand>(ThisType,
            (b, _, v) => ((ImageButton)b).command = (ICommand)v
        );
    public object? CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    public static readonly BindableProperty CommandParameterProperty =
        Utility.Create<object>(ThisType,
            (b, _, v) => ((ImageButton)b).commandParameter = v
        );
    protected ICommand? command;
    protected object? commandParameter;
    public event EventHandler? Clicked;
    public event EventHandler? Pressed;
    public event EventHandler? Released;
    #endregion
    public ImageButton()
	{
		InitializeComponent();
	}
    #region Gesture
    protected static readonly Color HoverColor = (Color)Application.Current!.Resources["HoverColor"];
    protected void OnPressed(object sender, EventArgs e) {
        Scale = 0.9;
        BackgroundColor = Colors.Transparent;
        Pressed?.Invoke(this, e);
    }
    protected void OnReleased(object sender, EventArgs e) {
        Scale = 1;
        BackgroundColor = HoverColor;
        Released?.Invoke(this, e);
    }
    protected void OnEntered(object sender, EventArgs e) {
        BackgroundColor = HoverColor;
    }
    protected void OnExited(object sender, EventArgs e) {
        BackgroundColor = Colors.Transparent;
    }
    protected virtual void OnClicked(object sender, EventArgs e) {
        BackgroundColor = Colors.Transparent;
        Clicked?.Invoke(this, e);
        command?.Execute(commandParameter);
    }
    protected void InvokeClicked(EventArgs e) {
        Clicked?.Invoke(this, e);
    }
    #endregion
}