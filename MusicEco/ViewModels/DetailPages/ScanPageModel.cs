using CommunityToolkit.Mvvm.Input;
using Domain.DataAccess;
using Domain.EventSystem;
using Domain.Models;
using MusicEco.ViewModels.Items;
using MusicEco.ViewModels.Pages;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels.DetailPages; 
public partial class ScanPageModel: PropertyObject {
    private bool busy;
    public bool ScanButtonVisible => !busy;
    public bool ProgressBarVisible => busy;
    public float Progress { get; set; }
    private readonly IScanner scanner;
    public ScanPageModel(IScanner scanner) {
        DataController = new([]);
        EventSystem.Connect<ScanFoldersChangedEventArgs>(OnScanFoldersChanged);
        this.busy = false;
        this.scanner = scanner;
    }
    private readonly ObservableCollectionController<ScanItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;
    private void OnScanFoldersChanged(object? s, ScanFoldersChangedEventArgs? arg) {
        LoadData().FireAndForgetAsync();
    }
    public async Task LoadData() {
        List<string> indexes = Enumerable.Range(0, GlobalData.ScanFolders.Count).Select(s => s.ToString()).ToList();
        await DataController.UpdateKeysAsync(indexes);
        await DataController.PageDown(0, AppSettingModel.Current.ListItems);
    }
    [RelayCommand]
    public void RemoveFolder(string indexStr) {
        List<string> folders = GlobalData.ScanFolders;
        folders.RemoveAt(int.Parse(indexStr));
        GlobalData.ScanFolders = folders;
    }
    [RelayCommand]
    public async Task AddFolder() {
        string? folderPath;
        folderPath = await SettingPageModel.OpenFolderPicker();
        if (folderPath == null) {
            return;
        }
        List<string> folders = GlobalData.ScanFolders;
        folders.Add(folderPath);
        GlobalData.ScanFolders = folders;
        await Task.CompletedTask;
    }
    [RelayCommand]
    public async Task ScanMusic() {
        if (this.busy) return;
        this.busy = true;
        OnPropertyChanged(nameof(ScanButtonVisible));
        OnPropertyChanged(nameof(ProgressBarVisible));
        Progress = 0;
        OnPropertyChanged(nameof(Progress));
        Progress<Tuple<int, int>> logProgress = new(value => {
            int current = value.Item1;
            int total = value.Item2;
            Progress = (float)current / total;
            OnPropertyChanged(nameof(Progress));
        });
#if WINDOWS
        ItemSource source = ItemSource.Windows;
#elif ANDROID
        ItemSource source = ItemSource.Androids;
#endif
        await scanner.ScanAsync(
            logProgress,
            GlobalData.ScanFolders,
            [".mp3"],
            true,
            source
            );
        var diff = scanner.CheckDiff();
        Debug.WriteLine($"Missing songs: {diff["missingSongs"].Count} New songs: {diff["newSongs"].Count}");
        scanner.Commit();
        await Task.CompletedTask;
        this.busy = false;
        OnPropertyChanged(nameof(ScanButtonVisible));
        OnPropertyChanged(nameof(ProgressBarVisible));
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
    [RelayCommand]
    public void DeleteAllData() {
        scanner.DeleteAllData();
    }
}
