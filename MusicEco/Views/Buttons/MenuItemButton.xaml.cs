
using MusicEco.SourceGeneration;
using MusicEco.Views.Items;
using System.Windows.Input;

namespace MusicEco.Views.Buttons;

public partial class MenuItemButton: ContentView, IMenuItemButton {
    private static readonly Type ThisType = typeof(MenuItemButton);
    [BindedProperty]
    public partial ICommand? Command { get; set; }
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
    [BindedProperty]
    public partial string Text { get; set; }
    public static readonly BindableProperty TextProperty
        = Utility.Create<string>(ThisType, string.Empty);
    public event EventHandler? Tapped;
    public MenuItemButton() {
        InitializeComponent();
    }

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e) {
        this.Tapped?.Invoke(this, e);
        this.Command?.Execute(this.BindingContext);
    }

    private void PointerGestureRecognizer_PointerEntered(object? sender, PointerEventArgs e) {
        this.BackgroundColor = DynamicColors.ButtonHighlightColor;
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