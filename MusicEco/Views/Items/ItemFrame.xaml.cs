using System.Windows.Input;

namespace MusicEco.Views.Items;

public partial class ItemFrame: Border {
    private static readonly Type ThisType = typeof(ItemFrame);
    public static readonly BindableProperty CommandProperty
        = Utility.Create<ICommand?>(ThisType);
    public ICommand? Command {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
    public event EventHandler? Tapped;
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
        if (Command != null) {
            if (Command.CanExecute(this.BindingContext)) {
                Command.Execute(this.BindingContext);
            }
        }
    }

    private void PointerGestureRecognizer_PointerEntered(object? sender, PointerEventArgs e) {
        Container.BackgroundColor = Utility.GetResource<Color>("ItemHighlightColor");
    }

    private void PointerGestureRecognizer_PointerExited(object? sender, PointerEventArgs e) {
        Container.BackgroundColor = Colors.Transparent;
    }
}