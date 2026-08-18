using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels.Items;

namespace MusicEco.ViewModels.Pages;

public partial class ExplorerTreePageViewModel: BasePageViewModel {
    private sealed record Query {
        public string RootFolder = string.Empty;
        public string CurrentFolder = string.Empty;
    }
    private const string PathSeparator
#if WINDOWS
        = "\\";
#else
        = "";
#endif
    private readonly Query _q;
    private readonly IFileService _fileService;
    private readonly IAudioService _audioService;
    private readonly IPlaybackService _playbackService;
    private readonly FolderStack _stack;
    public string FolderPath => _q.CurrentFolder; 
    public override PageRoute Route => PageRoute.ExplorerTree;
    public ObservableCollectionExtend<IUpdateble> Items { get; init; }
    public ExplorerTreePageViewModel(ILocalizationService localizationService, IAppSetting appSetting, IFileService fileService, IAudioService audioService, IPlaybackService playbackService) : base(localizationService, appSetting) {
        this._q = new();
        this._fileService = fileService;
        this._audioService = audioService;
        this._playbackService = playbackService;
        this._stack = new();
        this.Items = new();
    }
    public override async Task Refresh() {
        if (PathSeparator.Length == 0) {
            throw new InvalidOperationException();
        }
        NotifyFolderNavigation();
        OnPropertyChanged(nameof(FolderPath));
        string currentFolderPath = this._q.CurrentFolder;
        var rootPathLength = currentFolderPath.Length + PathSeparator.Length;
        var files = await this._fileService.Query(currentFolderPath);
        HashSet<string> folderPaths = [];
        List<FileEntryViewModel> fileItems = [];
        foreach(var file in files) {
            string relPath = file.Path[rootPathLength..];
            if (relPath.Contains(PathSeparator)) {
                string folderName = relPath.Split(PathSeparator)[0];
                string shortPath = currentFolderPath + PathSeparator + folderName;
                folderPaths.Add(shortPath);
            } else {
                FileEntryViewModel item = new(file.Hash, file.Path);
                fileItems.Add(item);
            }
        }
        List<FolderEntryViewModel> folderItems = [];
        foreach(var path in folderPaths) {
            FolderEntryViewModel item = new(path);
            folderItems.Add(item);
        }
        List<IUpdateble> totalItems = folderItems.OrderBy(f => f.Name).Cast<IUpdateble>().ToList();
        totalItems.AddRange(fileItems.OrderBy(f => f.Name));
        this.Items.Update(totalItems);
    }
    public override async Task OnNavigateTo(NavigateEventArgs e) {
        await base.OnNavigateTo(e);
        if (e.Query.TryGetValue("rootFolder", out var pathObj)) {
            if (pathObj is string path) {
                if (this._q.RootFolder != path) {
                    this._stack.Reset();
                    this._stack.ToFolder(path);
                }
                this._q.RootFolder = path;
                this._q.CurrentFolder = path;
                await Refresh();
            }
        }
        FireNavigated(e);
    }
    public override Task OnNavigatedFrom(NavigateEventArgs e) {
        return base.OnNavigatedFrom(e);
    }
    [RelayCommand]
    private async Task SelectFolder(FolderEntryViewModel? vm) {
        if (vm == null) {
            return;
        }
        this._q.CurrentFolder = vm.Path;
        this._stack.ToFolder(vm.Path);
        await Refresh();
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
        foreach(var fileHash in fileHashes) {
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
    private void NotifyFolderNavigation() {
        this.PreviousFolderCommand.NotifyCanExecute();
        this.NextFolderCommand.NotifyCanExecute();
        this.UpFolderCommand.NotifyCanExecute();
    }
    private bool CanPrevious() {
        return this._stack.CanPrevious();
    }
    [RelayCommand(CanExecute = nameof(CanPrevious))]
    private async Task PreviousFolder() {
        string path = this._stack.PreviousFolder();
        this._q.CurrentFolder = path;
        await Refresh();
    }
    private bool CanNext() {
        return this._stack.CanNext();
    }
    [RelayCommand(CanExecute = nameof(CanNext))]
    private async Task NextFolder() {
        string path = this._stack.NextFolder();
        this._q.CurrentFolder = path;
        await Refresh();
    }
    private bool CanUp() {
        string rootPath = this._q.RootFolder;
        string currentPath = this._q.CurrentFolder;
        return rootPath != currentPath && currentPath.Contains(rootPath);
    }
    [RelayCommand(CanExecute = nameof(CanUp))]
    private async Task UpFolder() {
        string currentPath = this._q.CurrentFolder;
        string upPath = System.IO.Path.GetDirectoryName(currentPath)!;
        this._q.CurrentFolder = upPath;
        this._stack.ToFolder(upPath);
        await Refresh();
    }
}
