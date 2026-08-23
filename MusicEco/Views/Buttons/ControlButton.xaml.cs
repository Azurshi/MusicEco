using MusicEco.SourceGeneration;
using System.Windows.Input;

namespace MusicEco.Views.Buttons;

public partial class ControlButton: ContentView {
    private static readonly Type ThisType = typeof(ControlButton);
    [BindableAutoGen]
    public static readonly BindableProperty CommandProperty
        = Utility.Create<ICommand?>(ThisType);
    [BindedProperty]
    public partial bool IsActive { get; set; }
    public static readonly BindableProperty IsActiveProperty
        = Utility.Create<bool>(ThisType, false,
            propertyChanged: (b, _, v) => {
                var This = (ControlButton)b;
                var value = (bool)v;
                This.RefreshState();
            });
    [BindedProperty]
    public partial string ActiveResourcePath { get; set; }
    public static readonly BindableProperty ActiveResourcePathProperty
        = Utility.Create<string>(ThisType, string.Empty,
            propertyChanged: (b, _, v) => {
                var This = (ControlButton)b;
                var value = (string)v;
                This.ActiveLabel.ResourcePath = value;
            });
    [BindedProperty]
    public partial string InactiveResourcePath { get; set; }
    public static readonly BindableProperty InactiveResourcePathProperty
        = Utility.Create<string>(ThisType, string.Empty,
            propertyChanged: (b, _, v) => {
                var This = (ControlButton)b;
                var value = (string)v;
                This.InactiveLabel.ResourcePath = value;
            });

    private void RefreshState() {
        this.ActiveLabel.IsVisible = this.IsActive;
        this.InactiveLabel.IsVisible = !this.IsActive;
    }
    public event EventHandler<TappedEventArgs>? Tapped;
    public ControlButton() {
        InitializeComponent();
        this.RefreshState();
    }
    private void OnPointerPressed(object? sender, PointerEventArgs e) {
        Scale = 0.9;
        BackgroundColor = Colors.Transparent;
    }

    private void OnPointerReleased(object? sender, PointerEventArgs e) {
        Scale = 1.0;
        BackgroundColor = DynamicColors.ButtonHighlightColor;
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e) {
        BackgroundColor = DynamicColors.ButtonHighlightColor;
    }

    private void OnPointerExited(object? sender, PointerEventArgs e) {
        BackgroundColor = Colors.Transparent;
    }

    private void OnTapped(object? sender, TappedEventArgs e) {
        Command?.Execute(this.IsActive);
        Tapped?.Invoke(this, e);
    }
}