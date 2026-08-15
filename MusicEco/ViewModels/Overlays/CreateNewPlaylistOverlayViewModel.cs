using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicEco.ViewModels.Overlays;

public partial class CreateNewPlaylistOverlayViewModel: ObservableObject {
    private readonly IPlaylistService _playlistService;
    // Weak Action to prevent ViewModel keep View alive
    private WeakReference<Action>? _closeRef;
    public AssemblyLocalization L { get; init; }
    public AsyncCommandExtend ConfirmCommand { get; init; }
    public SyncCommand CancelCommand { get; init; }
    private bool _initialized = false;
    private string _playlistName = string.Empty;
    public string PlaylistName {
        get => this._playlistName;
        set {
            if (this._playlistName != value) {
                this._playlistName = value;
                OnPropertyChanged();
                ConfirmCommand.NotifyCanExecute();
            }
        }
    }
    public async Task Initialize(Action close) {
        if (this._initialized) {
            return;
        }
        this._closeRef = new(close);
        var playlists = await this._playlistService.GetAll();
        foreach(var playlist in playlists) {
            this._existsNames.Add(playlist.Name);
        }
        this._initialized = true;
        this.ConfirmCommand.NotifyCanExecute();
    }
    private readonly HashSet<string> _existsNames;
    public CreateNewPlaylistOverlayViewModel(ILocalizationService localizationService, IPlaylistService playlistService) {
        this.L = localizationService.Get(typeof(BasePageViewModel));
        this._playlistService = playlistService;
        this._existsNames = [];
        this.ConfirmCommand = new(Create, CanCreate);
        this.CancelCommand = new(Cancel);

    }
    private bool CanCreate() {
        if (!this._initialized) {
            return false;
        }
        string name = this._playlistName.Trim();
        if (name.Length < Config.MinNameLength || this._existsNames.Contains(name)) {
            return false;
        }
        return true;
    }
    private async Task Create() {
        if (!this._initialized) {
            return;
        }
        string name = this._playlistName.Trim();
        var now = DateTime.UtcNow;
        AudioPlaylist playlist = new(name, now, now, DateTime.MinValue, []);
        await this._playlistService.Insert(playlist);
        if (this._closeRef!.TryGetTarget(out var target)) {
            target.Invoke();
        }
        else {
            throw new InvalidOperationException("View close Action already collected by GC");
        }
    }
    private void Cancel() {
        if (!this._initialized) {
            return;
        }
        if (this._closeRef!.TryGetTarget(out var target)) {
            target.Invoke();
        }
        else {
            throw new InvalidOperationException("View close Action already collected by GC");
        }
    }
}
