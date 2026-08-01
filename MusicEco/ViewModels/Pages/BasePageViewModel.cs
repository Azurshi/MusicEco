using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Pages;

public interface INavigationAware {
    public Task OnNavigateTo(NavigateEventArgs e);
    public Task OnNavigatedFrom(NavigateEventArgs e);
}
public interface ILocalizationAware {
    public AssemblyLocalization L { get; }
}

public abstract class BasePageViewModel: ObservableObject, INavigationAware, ILocalizationAware {
    public abstract PageRoute Route { get; }
    public AssemblyLocalization L { get; init; }
    private readonly ILocalizationService _localizationService;
    private bool _isActive = false;
    public bool IsActive {
        get => this._isActive;
        set {
            if (this._isActive != value) {
                this._isActive = value;
                OnPropertyChanged();
            }
        }
    }
    public BasePageViewModel(ILocalizationService localizationService) {
        EventSystem.Connect<RefreshEventArgs>(OnRefreshEvent);
        this._localizationService = localizationService;
        this.L = this._localizationService.Get(typeof(BasePageViewModel));
    }

    private void OnLanguageChanged(object? sender, EventArgs e) {
        OnPropertyChanged(nameof(L));
    }

    private async void OnRefreshEvent(object? sender, RefreshEventArgs e) {
        if (this._isActive) {
            await Refresh();
        }
    }
    public abstract Task Refresh();
    public virtual async Task OnNavigatedFrom(NavigateEventArgs e) {
        this._localizationService.LanguageChanged -= OnLanguageChanged;
    }

    public virtual async Task OnNavigateTo(NavigateEventArgs e) {
        this._localizationService.LanguageChanged += OnLanguageChanged;
        OnPropertyChanged(nameof(L));
    }
    protected void FireNavigated(NavigateEventArgs e) {
        NavigatedEventArgs args = new(e);
        EventSystem.Publish(this, args);
    }
}
