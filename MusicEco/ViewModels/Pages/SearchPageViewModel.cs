using MusicEco.Core;
using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.ViewModels.Items;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.Pages;

public partial class SearchPageViewModel: BasePageViewModel {
    public override PageRoute Route => PageRoute.Search;
    private readonly IAudioService _audioService;
    private string _searchText = string.Empty;
    public string SearchText {
        get => _searchText;
        set {
            if (this._searchText != value) {
                this._searchText = value;
                OnPropertyChanged();
                if (this._searchText.Trim().Length > 2) {
                    Refresh().FireAndForgetAsync();
                }
            }
        }
    }
    public ObservableCollection<AudioEntryViewModel> Items { get; init; }

    public SearchPageViewModel(ILocalizationService localizationService, IAudioService audioService) : base(localizationService) {
        this._audioService = audioService;
        this.Items = [];
    }
    public override async Task Refresh() {
        if (this._searchText.Trim().Length < 3) {
            return;
        }
        string query = this._searchText;
        var entries = await this._audioService.QueryEntry(query);
        this.Items.Clear();
        foreach(var entry in entries) {
            this.Items.Add(new(entry.Hash, entry.Title));
        }
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
}
