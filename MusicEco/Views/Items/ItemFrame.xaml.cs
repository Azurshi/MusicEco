using MusicEco.SourceGeneration;
using System.Windows.Input;

namespace MusicEco.Views.Items;

public partial class ItemFrame: Border {
    private static readonly Type ThisType = typeof(ItemFrame);
    [BindableAutoGen<ICommand>(IsNullable = true)]
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
    public static readonly BindableProperty ItemContentProperty
        = Utility.Create<View?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (ItemFrame)b;
                var value = (View?)v;
                This.Container.Content = value;
            });
    public View? ItemContent {
        get => (View?)GetValue(ItemContentProperty);
        set => SetValue(ItemContentProperty, value);
    }
    public event EventHandler? Tapped;
    public static readonly BindableProperty IsSelectedProperty
        = Utility.Create<bool>(ThisType, false,
            propertyChanged: (b, _, v) => {
                var This = (ItemFrame)b;
                var value = (bool)v;
                Color color;
                if (value) {
                    color = Utility.GetResource<Color>("SelectedItemBorderColor");
                } else {
                    color = Colors.Transparent;
                }
                This.Stroke = new SolidColorBrush(color);
            });
    public bool IsSelected {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    public ItemFrame() {
        InitializeComponent();
    }

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e) {
        Tapped?.Invoke(this, e);
        Command?.Execute(this.BindingContext);
    }

    private void PointerGestureRecognizer_PointerEntered(object? sender, PointerEventArgs e) {
        Container.BackgroundColor = Utility.GetResource<Color>("ItemHighlightColor");
    }

    private void PointerGestureRecognizer_PointerExited(object? sender, PointerEventArgs e) {
        Container.BackgroundColor = Colors.Transparent;
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