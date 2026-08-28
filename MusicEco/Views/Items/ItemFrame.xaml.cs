using MusicEco.SourceGeneration;
using MusicEco.Views.Buttons;
using System.Windows.Input;

namespace MusicEco.Views.Items;

public partial class ItemFrame: Border {
    private static readonly Type ThisType = typeof(ItemFrame);
    [BindableAutoGen]
    public static readonly BindableProperty CommandProperty
        = Utility.Create<ICommand?>(ThisType,
            propertyChanged: (b, oldValue, newValue) => {
                var This = (ItemFrame)b;
                var oldCommand = (ICommand?)oldValue;
                var newCommand = (ICommand?)newValue;
                oldCommand?.CanExecuteChanged -= This.NewCommand_CanExecuteChanged;
                newCommand?.CanExecuteChanged += This.NewCommand_CanExecuteChanged;
                This.RefreshCommandState();
            });
    [BindedProperty]
    public partial View? ItemContent { get; set; }
    public static readonly BindableProperty ItemContentProperty
        = Utility.Create<View?>(ThisType);
    [BindedProperty]
    public partial bool IsSelected { get; set; }
    public static readonly BindableProperty IsSelectedProperty
        = Utility.Create<bool>(ThisType, false,
            propertyChanged: (b, _, v) => {
                var This = (ItemFrame)b;
                var value = (bool)v;
                Color color;
                if (value) {
                    color = DynamicColors.SelectedBorderColor;
                } else {
                    color = Colors.Transparent;
                }
                This.Stroke = new SolidColorBrush(color);
            });
    [BindedProperty]
    public partial Color BorderColor { get; set; }
    public static readonly BindableProperty BorderColorProperty
        = Utility.Create<Color>(ThisType, Colors.Transparent);
    public event EventHandler? Tapped;

    public ItemFrame() {
        InitializeComponent();
    }

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e) {
        this.Tapped?.Invoke(this, e);
        this.Command?.Execute(this.BindingContext);
    }

    private void PointerGestureRecognizer_PointerEntered(object? sender, PointerEventArgs e) {
        this.Container.BackgroundColor = DynamicColors.HighLightColor;
    }

    private void PointerGestureRecognizer_PointerExited(object? sender, PointerEventArgs e) {
        this.Container.BackgroundColor = Colors.Transparent;
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
            this.Opacity = 0.5;
        }
    }
}