#if ANDROID
using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Platforms.Android;
using MusicEco.Platform;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

using Uri = Android.Net.Uri;
public partial class ExplorerTreePageViewModel {
    private sealed record Query {
        public Uri? RootFolder;
        public Uri? CurrentFolder;
    }
    private readonly Query _q;
    private readonly IFileService _fileService;
    private readonly IAudioService _audioService;
    private readonly IPlaybackService _playbackService;
    private readonly FolderStack<Uri> _stack;
    private readonly Dictionary<string, Uri> _parentMapping;
    //private readonly FolderStack _stack;
    public ExplorerTreePageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IFileService fileService, IAudioService audioService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._q = new();
        this._fileService = fileService;
        this._audioService = audioService;
        this._playbackService = playbackService;
        this._stack = new();
        this._parentMapping = [];
    }
    [RelayCommand]
    public override async Task Refresh() {
        NotifyFolderNavigation();
        //OnPropertyChanged(nameof(FolderPath));
        var currentFolder = this._q.CurrentFolder;
        if (currentFolder == null) {
            return;
        }
        string currentFolderPath = currentFolder.ToString()!;
        List<FolderEntryViewModel> folderItems = [];
        List<FileEntryViewModel> fileItems = [];
        foreach (var itemInfo in UriQuery.GetItemsInfo(currentFolder)) {
            if (itemInfo is FolderInfo folder) {
                this._parentMapping[folder.Uri.ToString()!] = currentFolder;
                FolderEntryViewModel item = new(folder.Uri.ToString()!, folder.Name, folder.Uri);
                folderItems.Add(item);
            }
            else if (itemInfo is MusicEco.Data.Platforms.Android.FileInfo file) {
                FileEntry? entry = await this._fileService.Get(itemInfo.Uri.ToString()!);
                if (entry != null) {
                    FileEntryViewModel item = new(entry.Hash, file.Path, file.Name);
                    fileItems.Add(item);
                }
            }
        }
        var files = await this._fileService.Query(currentFolderPath);
        HashSet<string> folderPaths = [];
        List<IUpdateble> totalItems = folderItems.OrderBy(f => f.Name).Cast<IUpdateble>().ToList();
        totalItems.AddRange(fileItems.OrderBy(f => f.Name));
        this.Items.Update(totalItems);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        if (e.Query.TryGetValue("rootFolder", out var pathObj)) {
            if (pathObj is string path) {
                this._q.RootFolder = UriUtility.GetUri(path);
                this._q.CurrentFolder = this._q.RootFolder;
                this._stack.Reset();
                this._stack.ToFolder(this._q.RootFolder!);
                this._parentMapping.Clear();
                this._parentMapping[this._q.CurrentFolder!.ToString()!] = this._q.RootFolder!; 
                await Refresh();
            }
        }
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
    private void NotifyFolderNavigation() {
        this.PreviousFolderCommand.NotifyCanExecute();
        this.NextFolderCommand.NotifyCanExecute();
        this.UpFolderCommand.NotifyCanExecute();
    }
    [RelayCommand]
    private async Task SelectFolder(FolderEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        this._q.CurrentFolder = vm.Uri;
        this._stack.ToFolder(vm.Uri);
        await this.Refresh();
    }
    [RelayCommand]
    private async Task SelectFile(FileEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        string queueName = $"Folder {this._q.CurrentFolder}";
        AudioEntry? selected = null;
        List<AudioEntry> audios = [];
        List<Hash256> fileHashes = this.Items.Items.OfType<FileEntryViewModel>().Select(f => f.FileHash).ToList();
        var audioMap = await this._audioService.GetEntry(fileHashes);
        foreach (var fileHash in fileHashes) {
            if (audioMap.TryGetValue(fileHash, out var entry)) {
                if (fileHash == vm.FileHash) {
                    selected = entry;
                }
                audios.Add(entry);
            }
        }
        if (selected == null) {
            throw new InvalidOperationException();
        }
        await this._playbackService.PlayQueue(queueName, audios, selected, this);
    }
    private bool CanPrevious() {
        return this._stack.CanPrevious();
    }
    [RelayCommand(CanExecute = nameof(CanPrevious))]
    private async Task PreviousFolder() {
        var uri = this._stack.PreviousFolder();
        this._q.CurrentFolder = uri;
        await Refresh();
    }
    private bool CanNext() {
        return this._stack.CanNext();
    }
    [RelayCommand(CanExecute = nameof(CanNext))]
    private async Task NextFolder() {
        var uri = this._stack.NextFolder();
        this._q.CurrentFolder = uri;
        await Refresh();
    }
    private bool CanUp() {
        if (this._q.RootFolder == null || this._q.CurrentFolder == null) {
            return false;
        }
        if (this._q.RootFolder != this._q.CurrentFolder) {
            return true;
        }
        else {
            return false;
        }
    }
    [RelayCommand(CanExecute = nameof(CanUp))]
    private async Task UpFolder() {
        if (this._q.CurrentFolder == null) {
            return;
        }
        var uri = this._parentMapping[this._q.CurrentFolder.ToString()!];
        this._q.CurrentFolder = uri;
        this._stack.ToFolder(uri);
        await Refresh();
    }
}
#endif
