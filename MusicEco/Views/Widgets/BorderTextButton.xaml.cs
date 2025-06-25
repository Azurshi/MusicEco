using System.Windows.Input;

namespace MusicEco.Views.Widgets;

public partial class BorderTextButton : Border {
    #region Binding
    private static readonly Type ThisType = typeof(BorderTextButton);
    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly BindableProperty CommandProperty =
        Utility.Create<ICommand>(ThisType,
            (b, _, v) => ((BorderTextButton)b).command = (ICommand)v
        );
    public object? CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    public static readonly BindableProperty CommandParameterProperty =
        Utility.Create<object>(ThisType,
            (b, _, v) => ((BorderTextButton)b).commandParameter = v
        );
    private ICommand? command;
    private object? commandParameter;
    public event EventHandler? Clicked;

    public static readonly BindableProperty TextProperty =
        Utility.Create<string>(ThisType,
            (b, _, v) => ((BorderTextButton)b).InnerLabel.Text = (string)v
        );
    public string Text {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    public static readonly BindableProperty TextAlignProperty =
        Utility.Create<TextAlignment>(ThisType,
            (b, _, v) => ((BorderTextButton)b).InnerLabel.HorizontalTextAlignment = (TextAlignment)v
        );
    public TextAlignment TextAlign {
        get => (TextAlignment)GetValue(TextAlignProperty);
        set => SetValue(TextAlignProperty, value);
    }
    #endregion
    public BorderTextButton() {
        InitializeComponent();
    }
    #region Signal
    private static readonly Color HoverColor = (Color)Application.Current!.Resources["HoverColor"];
    private Color PreviousColor = Colors.Transparent;
    private void OnEntered(object sender, EventArgs e) {
        PreviousColor = this.BackgroundColor;
        BackgroundColor = HoverColor;
    }
    private void OnExited(object sender, EventArgs e) {
        BackgroundColor = PreviousColor;
    }
    private void OnClicked(object sender, EventArgs e) {
        BackgroundColor = PreviousColor;
        Clicked?.Invoke(this, e);
        command?.Execute(commandParameter);
    }
    #endregion
}