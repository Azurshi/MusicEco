using System.Diagnostics;
using System.Windows.Input;

namespace MusicEco.Views.Widgets;

public partial class TextButton : Label {
    #region Binding
    private static readonly Type ThisType = typeof(TextButton);
    public ICommand Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public static readonly BindableProperty CommandProperty = Utility.Create<ICommand>(ThisType);
    public object? CommandParameter {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
    public static readonly BindableProperty CommandParameterProperty = Utility.Create<object>(ThisType);
    public bool Available {
        get => (bool)GetValue(AvailableProperty);
        set => SetValue(AvailableProperty, value);
    }
    public static readonly BindableProperty AvailableProperty = Utility.Create<bool>(ThisType,
        propertyChanged: (b, _, v) => { 
            TextButton This = (TextButton)b;
            bool value = (bool)v;
            if (value) {
                This.TextColor = This.PreviousTextColor;
            } else {
                This.TextColor = DisabledColor;
            }
        },
        defaultValue: true
        );
    #endregion
    public event EventHandler<TappedEventArgs>? Clicked;
    private static readonly Color DisabledColor = (Color)Application.Current!.Resources["DisabledColor"];
    private Color PreviousTextColor;
    public TextButton() {
        InitializeComponent();
        PreviousTextColor = this.TextColor;
    }
    #region Signal
    private void OnClicked(object sender, TappedEventArgs e) {
        if (Available) {
            Clicked?.Invoke(this, e);
            Command?.Execute(CommandParameter);
        } else {
            Debug.WriteLine("Not available");
        }
    }
    #endregion
}