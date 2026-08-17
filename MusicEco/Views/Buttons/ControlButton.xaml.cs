using MusicEco.SourceGeneration;
using System.Windows.Input;

namespace MusicEco.Views.Buttons;

public partial class ControlButton: Grid {
    private static readonly Type ThisType = typeof(ControlButton);
    [BindableAutoGen<ICommand>(IsNullable = true)]
    public static readonly BindableProperty CommandProperty
        = Utility.Create<ICommand?>(ThisType);
    public static readonly BindableProperty IsActivateProperty
        = Utility.Create<bool>(ThisType, false,
            propertyChanged: (b, _, v) => {
                var This = (ControlButton)b;
                This.RefreshState();
            });
    public bool IsActivate {
        get => (bool)GetValue(IsActivateProperty);
        set => SetValue(IsActivateProperty, value);
    }
    public static readonly BindableProperty ActivateImageSourceProperty
        = Utility.Create<ImageSource?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (ControlButton)b;
                var value = (ImageSource?)v;
                This.ActivateImage.Source = value;
            });
    public ImageSource? ActivateImageSource {
        get => (ImageSource?)GetValue(ActivateImageSourceProperty);
        set => SetValue(ActivateImageSourceProperty, value);
    }
    public static readonly BindableProperty DeactivateImageSourceProperty
        = Utility.Create<ImageSource?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (ControlButton)b;
                var value = (ImageSource?)v;
                This.DeactivateImage.Source = value;
            });
    public ImageSource? DeactivateImageSource {
        get => (ImageSource?)GetValue(DeactivateImageSourceProperty);
        set => SetValue(DeactivateImageSourceProperty, value);
    }
    private void RefreshState() {
        this.ActivateImage.IsVisible = this.IsActivate;
        this.DeactivateImage.IsVisible = !this.IsActivate;
    }
    public ControlButton() {
        InitializeComponent();
        RefreshState();
    }
    private void OnPointerPressed(object? sender, PointerEventArgs e) {
        Scale = 0.9;
        BackgroundColor = Colors.Transparent;
    }

    private void OnPointerReleased(object? sender, PointerEventArgs e) {
        Scale = 1.0;
        BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e) {
        BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void OnPointerExited(object? sender, PointerEventArgs e) {
        BackgroundColor = Colors.Transparent;
    }

    private void OnTapped(object? sender, TappedEventArgs e) {
        Command?.Execute(IsActivate);
    }
}