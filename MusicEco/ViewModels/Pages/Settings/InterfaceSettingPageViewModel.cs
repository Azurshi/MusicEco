using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Settings;

public partial class InterfaceSettingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.InterfaceSetting;
    private readonly IStyleService _styleService;
    private readonly IAppInterfaceService _interfaceService;
    public IReadOnlyList<ThemeViewModel> Themes { get; private set; }
    public string CurrentThemeName { get; private set; }
    private readonly Dictionary<string, float> _map = new() {
        ["50%"] = 0.5f,
        ["75%"] = 0.75f,
        ["100%"] = 1f,
        ["125%"] = 1.25f,
        ["150%"] = 1.5f,
        ["175%"] = 1.75f,
        ["200%"] = 2f
    };
    public List<string> Scales => _map.Keys.ToList();
    public string SelectedScale {
        get {
            var currentScale = this._interfaceService.GetScale();
            string currentValue = string.Empty;
            foreach(var (key, value) in this._map) {
                if (value == currentScale) {
                    currentValue = key;
                }
            }
            return currentValue;
        }
        set {
            float scaleValue = this._map[value];
            this._interfaceService.SetScale(scaleValue);
            OnPropertyChanged();
        }
    }
    public InterfaceSettingPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IStyleService styleService, IAppInterfaceService appInterfaceService) : base(localizationService, appSetting) {
        this._styleService = styleService;
        this._interfaceService = appInterfaceService;
        this.Themes = [];
        this.CurrentThemeName = string.Empty;
    }
    public override async Task Refresh() {
        var selectedThemeId = this._styleService.GetCurrentThemeId();
        this.CurrentThemeName = this._styleService.GetCurrentThemeName();
        OnPropertyChanged(nameof(CurrentThemeName));
        var themes = this._styleService.GetAll();
        List<ThemeViewModel> items = [];
        foreach(var theme in themes) {
            ThemeViewModel item = new(theme.Id, theme.Name) {
                Selected = selectedThemeId == theme.Id
            };
            items.Add(item);
        }
        this.Themes = items;
        OnPropertyChanged(nameof(Themes));
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        await Refresh();
        FireNavigated(e);
        this._styleService.ThemeChanged += this.StyleService_ThemeChanged;
    }

    private async void StyleService_ThemeChanged(object? sender, EventArgs e) {
        await Refresh();
    }

    public override async Task OnNavigatedFrom(NavigateEventArgs e) {
        await base.OnNavigatedFrom(e);
        this._styleService.ThemeChanged -= this.StyleService_ThemeChanged;
    }
    private bool CanSelectTheme(ThemeViewModel? vm) {
        if (vm == null) {
            return false;
        }
        return this._styleService.GetCurrentThemeId() != vm.ThemeId;
    }
    [RelayCommand(CanExecute = nameof(CanSelectTheme))]
    private void SelectTheme(ThemeViewModel? vm) {
        if (vm == null) {
            return;
        }
        this._styleService.SetTheme(vm.ThemeId);
    }
}
