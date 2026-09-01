using MusicEco.Core;
namespace MusicEco.Views.Shell;


public partial class NavigationBar: ContentView {
    private readonly IAppInterfaceService _interfaceService;
    public NavigationBar(IAppInterfaceService appInterfaceService) {
        InitializeComponent();
        this._interfaceService = appInterfaceService;
        this._interfaceService.OrientationChanged += this.InterfaceService_OrientationChanged;
    }

    private void InterfaceService_OrientationChanged(object? sender, OrientationItem e) {
        LoadLayout();
    }
    private void LoadLayout() {
        DisplayOrientation orientation = this._interfaceService.GetOrientation().Orientation;
        Grid layout = orientation switch {
            DisplayOrientation.Landscape => this.LoadTemplate<Grid>("LandscapeLayout"),
            DisplayOrientation.Portrait => this.LoadTemplate<Grid>("PortraitLayout"),
            _ => throw new ValueNotExistsExeption()
        };
        if (this.Content is Element oldContent) {
            foreach(var child in oldContent.WalkChildrenRecursive(true)) {
                if (child is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
        }
        this.Content = layout;
    }
}