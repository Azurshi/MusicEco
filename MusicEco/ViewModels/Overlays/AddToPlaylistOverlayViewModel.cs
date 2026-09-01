using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Overlays;

public partial class AddToPlaylistOverlayViewModel: BaseOverlayViewModel {
    private readonly IPlaylistService _playlistService;
    // Weak Action to prevent ViewModel keep View alive
    private WeakReference<Action>? _closeRef;
    public IReadOnlyList<PlaylistItemViewModel> Items { get; private set; }
    private Hash256? _selectedHash;
    private readonly Dictionary<DateTime, bool> _canSelectMap;
    private bool _initialized = false;
    public AddToPlaylistOverlayViewModel(ILocalizationService localizationService, IPlaylistService playlistService): base(localizationService) {
        this._playlistService = playlistService;
        this.Items = [];
        this._canSelectMap = [];
    }
    public async Task Initialize(Hash256 fileHash, Action close) {
        this._closeRef = new(close);
        this._selectedHash = fileHash;
        var playlists = await this._playlistService.GetAll();
        List<PlaylistItemViewModel> items = [];
        foreach(var playlist in playlists.OrderBy(p => p.Name)) {
            PlaylistItemViewModel item = new(playlist.CreationTime, playlist.ModifiedTime, playlist.Name);
            items.Add(item);
            bool contain = playlist.Audios.Select(a => a.Hash).Contains(fileHash);
            this._canSelectMap[playlist.CreationTime] = !contain;
        }
        this.Items = items;
        OnPropertyChanged(nameof(Items));
        this._initialized = true;
        SelectItemCommand.NotifyCanExecute();
    }
    private bool CanSelectItem(PlaylistItemViewModel? vm) {
        if (!this._initialized || vm == null || this._selectedHash == null) {
            return false;
        }
        return this._canSelectMap[vm.CreationTime];
    }
    [RelayCommand(CanExecute = nameof(CanSelectItem))]
    private async Task SelectItem(PlaylistItemViewModel? vm) {
        if (!this._initialized || vm == null || this._selectedHash == null) {
            return;
        }
        var playlist = await this._playlistService.Get(vm.CreationTime);
        if (playlist != null) {
            var audios = playlist.Audios.Append(new(this._selectedHash.Value, string.Empty)).ToList();
            playlist = playlist.WithAudios(audios).WithModifyNow();
            await this._playlistService.Update(playlist);
            if (this._closeRef!.TryGetTarget(out var target)) {
                target.Invoke();
            }
            else {
                throw new InvalidOperationException("View close Action already collected by GC");
            }
        }
    }
}
