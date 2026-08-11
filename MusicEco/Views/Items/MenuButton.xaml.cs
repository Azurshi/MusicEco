using MusicEco.Services;
using MusicEco.Views.Shell;
using System.Numerics;
using System.Windows.Input;

namespace MusicEco.Views.Items;

public partial class MenuButton: ContentView {
    private static readonly Type ThisType = typeof(MenuButton);
    public BindableProperty MenuTemplateProperty = Utility.Create<DataTemplate?>(ThisType);
    public DataTemplate? MenuTemplate {
        get => (DataTemplate?)GetValue(MenuTemplateProperty);
        set => SetValue(MenuTemplateProperty, value);
    }
    private readonly IOverlayService _overlayService;
    public MenuButton() {
        InitializeComponent();
        this._overlayService = AppLifeCycle.Provider.GetRequiredService<IOverlayService>();
    }

    private void OnTapped(object sender, TappedEventArgs e) {
        var position = e.GetPosition(AppLifeCycle.Provider.GetRequiredService<AppOverlay>());
        if (position != null) {
            HandleExecute(new(((float)position.Value.X), ((float)position.Value.Y)));
        }
    }

    private void OnPointerEntered(object sender, PointerEventArgs e) {
        BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void OnPointerExited(object sender, PointerEventArgs e) {
        BackgroundColor = Colors.Transparent;
    }
    private void HandleExecute(Vector2 position) {
        if (this.MenuTemplate != null) {
            Layout menu = (Layout)MenuTemplate.CreateContent();
            menu.BindingContext = this.BindingContext;
            MenuFrame container = new();
            container.Assign(menu);
            this._overlayService.ShowFixed(position, container);
        }
    }
}