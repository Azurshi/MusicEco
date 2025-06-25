using MusicEco.Views.Components;
using MusicEco.Views.PageExtensions;
using System.Windows.Input;

namespace MusicEco.Views.Buttons;
public class GetBasePageEventArgs: EventArgs {
    public IBasePage? Page { get; set; }
}
/// <summary>
/// Auto bind ItemModel Key as CommandParameter
/// </summary>
public partial class BaseButton : Label {
    #region Binding
    private static readonly Type ThisType = typeof(BaseButton);
    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly BindableProperty CommandProperty =
        Utility.Create<ICommand>(ThisType,
            (b, _, v) => ((BaseButton)b).command = (ICommand)v
        );
    public object? CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    public static readonly BindableProperty CommandParameterProperty =
        Utility.Create<object>(ThisType,
            (b, _, v) => {
                ((BaseButton)b).commandParameter = v;
                ((BaseButton)b).CommandParameterChanged?.Invoke(b, EventArgs.Empty);
            }
        );
    protected ICommand? command;
    protected object? commandParameter;
    protected EventHandler? CommandParameterChanged;
    #endregion
    public event EventHandler<GetBasePageEventArgs>? GetPageEventHandler;
    public BaseButton() {
        InitializeComponent();
        PreviousColor = BackgroundColor;
    }
    #region Signal
    private static readonly Color HoverColor = (Color)Application.Current!.Resources["HoverColor"];
    protected Color PreviousColor;
    protected virtual Color HightLightColor => HoverColor;
    protected void OnEntered(object sender, EventArgs e) {
        PreviousColor = this.BackgroundColor;
        BackgroundColor = HightLightColor;
    }
    protected void OnExited(object sender, EventArgs e) {
        BackgroundColor = PreviousColor;
    }
    /// <summary>
    /// Beware of memory leak
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnClicked(object sender, TappedEventArgs e) {
        BackgroundColor = PreviousColor;
        command?.Execute(commandParameter);
        GetBasePageEventArgs arg = new();
        GetPageEventHandler?.Invoke(this, arg);
        IBasePage? basePage = arg.Page;
        if (basePage != null) {
            basePage.PageOverlay.Stop();
        }
    }
    protected void InvokeGetPage(GetBasePageEventArgs arg) {
        GetPageEventHandler?.Invoke(this, arg);
    }
    #endregion
}