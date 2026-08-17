using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;

namespace MusicEco.ViewModels.Pages;

public interface INavigationAware {
    public Task OnNavigateTo(NavigateEventArgs e);
    public Task OnNavigatedFrom(NavigateEventArgs e);
}
public interface ILocalizationAware {
    public AssemblyLocalization L { get; }
}

public abstract partial class BasePageViewModel: ObservableObject, INavigationAware, ILocalizationAware {
    public abstract PageRoute Route { get; }
    public AssemblyLocalization L { get; init; }
    protected readonly ILocalizationService _localizationService;
    protected readonly IAppSetting _setting;
    [ObservableProperty]
    public partial bool IsActive { get; set; }
    public BasePageViewModel(ILocalizationService localizationService, IAppSetting setting) {
        EventSystem.Connect<RefreshEventArgs>(OnRefreshEvent);
        this._localizationService = localizationService;
        this.L = this._localizationService.Get(typeof(BasePageViewModel));
        this._setting = setting;
    }

    private void OnLanguageChanged(object? sender, EventArgs e) {
        OnPropertyChanged(nameof(L));
    }

    private async void OnRefreshEvent(object? sender, RefreshEventArgs e) {
        if (this.IsActive) {
            await Refresh();
        }
    }
    public abstract Task Refresh();
    public virtual async Task OnNavigatedFrom(NavigateEventArgs e) {
        this._localizationService.LanguageChanged -= OnLanguageChanged;
        this.IsActive = false;
    }

    public virtual async Task OnNavigateTo(NavigateEventArgs e) {
        this._localizationService.LanguageChanged += OnLanguageChanged;
        OnPropertyChanged(nameof(L));
        this.IsActive = true;
    }
    protected void FireNavigated(NavigateEventArgs e) {
        NavigatedEventArgs args = new(e);
        EventSystem.Publish(this, args);
    }
}
