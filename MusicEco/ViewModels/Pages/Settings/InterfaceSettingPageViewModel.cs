using MusicEco.Core.Services;

namespace MusicEco.ViewModels.Pages.Settings;

public partial class InterfaceSettingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.InterfaceSetting;
    private readonly IAppInterfaceService _interfaceService;
    private string  GetPlatformTitle(string fieldName) {
#if ANDROID
        return this.L[fieldName];
#else
        return string.Empty;
#endif
    }
    public string ThemePickerTitle => this.GetPlatformTitle("Setting_Interface_Theme");
    public IReadOnlyList<ThemeItemViewModel> Themes { get; init; }
    public ThemeItemViewModel CurrentTheme {
        get {
            var current = this._interfaceService.GetTheme();
            foreach(var theme in this.Themes) {
                if (theme.ThemeId == current.Id) {
                    return theme;
                }
            }
            throw new KeyNotFoundException();
        }
        set {
            if (value != null) {
                this._interfaceService.SetTheme(value.ThemeId);
                OnPropertyChanged();
            }
        }
    }
    public string ScalePickerTitle => this.GetPlatformTitle("Setting_Interface_Scale");
    public IReadOnlyList<ScaleItemViewModel> Scales { get; init; }
    public ScaleItemViewModel CurrentScale {
        get {
            var current = this._interfaceService.GetScale();
            foreach(var scale in this.Scales) {
                if (scale.Value == current.Value) {
                    return scale;
                }
            }
            throw new KeyNotFoundException();
        }
        set {
            this._interfaceService.SetScale(value.Value);
            OnPropertyChanged();
        }
    }
    public string OrientationPickerTitle => this.GetPlatformTitle("Setting_Interface_Orientation");
    public IReadOnlyList<OrientationItemViewModel> Orientations { get; init; }
    public OrientationItemViewModel CurrentOrientation {
        get {
            var current = this._interfaceService.GetOrientation();
            foreach(var orientation in this.Orientations) {
                if (orientation.Value == current.Orientation) {
                    return orientation;
                }
            }
            throw new KeyNotFoundException();
        }
        set {
            this._interfaceService.SetOrientation(value.Value);
            OnPropertyChanged();
        }
    }
    public InterfaceSettingPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IAppInterfaceService appInterfaceService) : base(localizationService, appSetting) {
        this._interfaceService = appInterfaceService;
        this.Themes = [];

        // Initialize
        this.Themes = this._interfaceService.GetThemes()
            .Select(item => new ThemeItemViewModel(item.Id, item.Text))
            .ToList();
        this.Scales = this._interfaceService.GetScales()
            .Select(item => new ScaleItemViewModel(item.Value, item.Text))
            .ToList();
        this.Orientations = this._interfaceService.GetOrientations()
            .Select(item => new OrientationItemViewModel(item.Orientation, item.Text))
            .ToList();
    }
    public override async Task Refresh() {
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        await Refresh();
        FireNavigated(e);
        this._interfaceService.ThemeChanged += this.OnThemeChanged;
    }

    private async void OnThemeChanged(object? sender, ThemeItem e) {
        OnPropertyChanged(nameof(CurrentTheme));
    }

    public override async Task OnNavigatedFrom(NavigateEventArgs e) {
        await base.OnNavigatedFrom(e);
        this._interfaceService.ThemeChanged -= this.OnThemeChanged;
    }
}
