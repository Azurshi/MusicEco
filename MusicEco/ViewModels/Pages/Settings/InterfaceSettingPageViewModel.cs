using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages.Settings;

public partial class InterfaceSettingPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.InterfaceSetting;
    private readonly IStyleService _styleService;
    public IReadOnlyList<ThemeViewModel> Themes { get; private set; }
    public SyncCommandExtend<ThemeViewModel> SelectThemeCommand { get; init; }
    public string CurrentThemeName { get; private set; }
    public InterfaceSettingPageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IStyleService styleService) : base(localizationService, appSetting) {
        this._styleService = styleService;
        this.Themes = [];
        this.CurrentThemeName = string.Empty;
        this.SelectThemeCommand = new(SelectTheme, CanSelectTheme);
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
    private void SelectTheme(ThemeViewModel? vm) {
        if (vm == null) {
            return;
        }
        this._styleService.SetTheme(vm.ThemeId);
    }
}
