using MusicEco.SourceGeneration;
using MusicEco.Views.Buttons;
using System.Windows.Input;

namespace MusicEco.Views.Controls;

public enum ActiveDisplayMode {
    SingleTint,
    DualTint
}

public partial class SKImageButton: SKImageLabel, IDisposable {
    private static readonly Type ThisType = typeof(SKImageButton);
    [BindedProperty]
    public partial Color ActiveTintColor { get; set; }
    public static readonly BindableProperty ActiveTintColorProperty
        = Utility.Create<Color>(ThisType, Colors.Blue,
            propertyChanged: (b, _, v) => {
                var This = (SKImageButton)b;
                if (This.DisplayMode == ActiveDisplayMode.DualTint) {
                    This.RefreshState();
                }
            });
    [BindedProperty]
    public partial Color InactiveTintColor { get; set; }
    public static readonly BindableProperty InactiveTintColorProperty
        = Utility.Create<Color>(ThisType, Colors.Red,
            propertyChanged: (b, _, v) => {
                var This = (SKImageButton)b;
                if (This.DisplayMode == ActiveDisplayMode.DualTint) {
                    This.Opacity = 1.0;
                    This.RefreshState();
                }
            });
    [BindedProperty]
    public partial ActiveDisplayMode DisplayMode { get; set; }
    public static readonly BindableProperty DisplayModeProperty
        = Utility.Create<ActiveDisplayMode>(ThisType, ActiveDisplayMode.SingleTint,
            propertyChanged: (b, _, v) => {
                var This = (SKImageButton)b;
                This.RefreshState();
            });
    [BindedProperty]
    public partial ICommand? Command { get; set; }
    public static readonly BindableProperty CommandProperty
        = Utility.Create<ICommand?>(ThisType, null,
            propertyChanged: (b, oldV, newV) => {
                var This = (SKImageButton)b;
                var oldValue = (ICommand?)oldV;
                oldValue?.CanExecuteChanged -= This.OnCanExecuteChanged;
                var newValue = (ICommand?)newV;
                newValue?.CanExecuteChanged += This.OnCanExecuteChanged;
                This.RefreshState();
            });
    [BindedProperty]
    public partial object? CommandParameter { get; set; }
    public static readonly BindableProperty CommandParameterProperty
        = Utility.Create<object?>(ThisType, null);
    public event EventHandler<TappedEventArgs>? Tapped;
    public SKImageButton() {
        InitializeComponent();
        this.Option = SamplingOptions.Trilinear;
        this.MaxSize = Config.MaxIconButtonSize;
    }
    ~SKImageButton() {
        this.Command?.CanExecuteChanged -= this.OnCanExecuteChanged;
    }
    public override void Dispose() {
        base.Dispose();
        this.Command?.CanExecuteChanged -= this.OnCanExecuteChanged;
        this.Command = null;
        GC.SuppressFinalize(this);
    }
    private void OnCanExecuteChanged(object? sender, EventArgs e) {
        this.RefreshState();
    }
    private void RefreshState() {
        bool canExecute = false;
        if (this.Command != null) {
            canExecute = this.Command.CanExecute(this.CommandParameter);
        }
        if (DisplayMode == ActiveDisplayMode.SingleTint) {
            if (canExecute) {
                this.Opacity = 1;
            }
            else {
                this.Opacity = 0.75;
            }
        }
        else if (DisplayMode == ActiveDisplayMode.DualTint) {
            if (canExecute) {
                this.TintColor = this.ActiveTintColor;
            }
            else {
                this.TintColor = this.InactiveTintColor;
            }
        }
    }
    public void ChangeToActiveState() {
        if (DisplayMode == ActiveDisplayMode.SingleTint) {
            this.Opacity = 1;
        }
        else if (DisplayMode == ActiveDisplayMode.DualTint) {
            this.TintColor = this.ActiveTintColor;
        }
    }
    public void ChangeToInactiveState() {
        if (DisplayMode == ActiveDisplayMode.SingleTint) {
            this.Opacity = 0.75;
        }
        else if (DisplayMode == ActiveDisplayMode.DualTint) {
            this.TintColor = this.InactiveTintColor;
        }
    }
    private void OnTapped(object sender, TappedEventArgs e) {
        this.Tapped?.Invoke(this, e);
        this.Command?.Execute(this.CommandParameter);
    }

    private void OnPointerEntered(object sender, PointerEventArgs e) {
        this.BackgroundColor = DynamicColors.ButtonHighlightColor;
    }

    private void OnPointerExited(object sender, PointerEventArgs e) {
        this.BackgroundColor = Colors.Transparent;
    }
}
