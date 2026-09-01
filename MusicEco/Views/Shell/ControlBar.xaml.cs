using MusicEco.Core;
using MusicEco.Services;
using MusicEco.ViewModels.Shell;
using System.Numerics;

namespace MusicEco.Views.Shell;

public partial class ControlBar: ContentView {
    private readonly IAppInterfaceService _interfaceService;
    public ControlBar(ControlBarViewModel viewModel, IAppInterfaceService interfaceService) {
        InitializeComponent();
        this.BindingContext = viewModel;
        this._interfaceService = interfaceService;
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
            foreach (var child in oldContent.WalkChildrenRecursive(true)) {
                if (child is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
        }
        this.Content = layout;
    }

    private void ControlButton_Tapped(object sender, TappedEventArgs e) {
        if (this.BindingContext is ControlBarViewModel viewModel) {
            var position = e.GetPosition(AppLifeCycle.Provider.GetRequiredService<AppOverlay>());
            if (position != null) {
                viewModel.ChangeVolumeCommand.Execute(new Vector2((float)position.Value.X, (float)position.Value.Y));
            }
        }
    }
}