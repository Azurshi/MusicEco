using CommunityToolkit.Mvvm.Input;
using Domain.Models;
using MusicEco.Settings;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Widgets;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels.Pages;
public partial class ExplorerPageModel : PropertyObject, IServiceAccess, IQueryAttributable {
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if (query.ContainsKey("id")) {
            long id = Convert.ToInt64(query["id"]);
            LoadData(id).FireAndForgetAsync();
        }
    }
    public ExplorerPageModel() {
        DataController = new([]);
    }
    private readonly ObservableCollectionController<ExplorerItemModel> DataController;
    public ObservableCollection<BaseItem> Data => DataController.Target;
    private long _folderId = -1;
    public async Task LoadData(long folderId) {
        _folderId = folderId;
        if (folderId != -1) {
            IFolderModel? folderModel = IServiceAccess.ModelGetter.Folder(folderId);
            if (folderModel != null) {
                List<string> folderIds = folderModel.ChildFolders.Select(s => s.Id.ToString()).ToList();
                List<string> fileIds = folderModel.ChildFiles.Select(s => s.Id.ToString()).ToList();
                List<string> totalIds = [];
                foreach (var id in folderIds) {
                    totalIds.Add(id);
                }
                foreach (var id in fileIds) {
                    totalIds.Add(id);
                }
                await DataController.UpdateKeysAsync(totalIds);
            }
        }
        else {
            List<string> folderIds = IServiceAccess.ModelQuery.Folder("root").Select(s => s.Id.ToString()).ToList();
            List<string> fileIds = IServiceAccess.ModelQuery.File("root").Select(s => s.Id.ToString()).ToList();
            List<string> totalIds = [];
            foreach (var id in folderIds) {
                totalIds.Add(id);
            }
            foreach (var id in fileIds) {
                totalIds.Add(id);
            }
            await DataController.UpdateKeysAsync(totalIds);
        }
        await DataController.PageDown(0, AppSettingModel.Current.ListItems);
    }

    [RelayCommand]
    public async Task FolderSelect(object idObj) {
        long id = long.Parse((string)idObj);
        //Debug.WriteLine(id);
        GlobalData.Explorer.NavigateSelect(id);
        await LoadData(id);
    }
    [RelayCommand]
    public void FileSelect(object idObj) {
        long fileId = long.Parse((string)idObj);
        Debug.WriteLine(fileId);
        ISongModel? songModel = IServiceAccess.ModelQuery.SongByFileId(fileId);
        IFolderModel? folder = IServiceAccess.ModelGetter.Folder(_folderId);
        if (folder == null || songModel == null) return;
        string queueName = $"Folder {folder.Name}";
        List<IFileModel> files = folder.ChildFiles;
        List<ISongModel> songs = [];
        foreach (var file in files) {
            ISongModel? song = IServiceAccess.ModelQuery.SongByFileId(file.Id);
            if (song != null) {
                songs.Add(song);
            }
        }
        IServiceAccess.PlayQueue(songModel.Id, songs, queueName);
    }
    [RelayCommand]
    public async Task NavigateBackward() {
        long? folderId = GlobalData.Explorer.NavigateBackward();
        if (folderId != null) {
            await LoadData(folderId.Value);
        }
    }
    [RelayCommand]
    public async Task NavigateForward() {
        long? folderId = GlobalData.Explorer.NavigateForward();
        if (folderId != null) {
            await LoadData(folderId.Value);
        }
    }
    [RelayCommand]
    public async Task NavigateUp() {
        long? folderId = GlobalData.Explorer.NavigateUp();
        if (folderId != null) {
            await LoadData(folderId.Value);
        }
    }
    [RelayCommand]
    public async Task NavigateRefresh() {
        long? folderId = GlobalData.Explorer.NavigateRefresh();
        if (folderId != null) {
            await LoadData(folderId.Value);
        }
    }
    [RelayCommand]
    public async Task NavigateHome() {
        long folderId = GlobalData.Explorer.NavigateClear();
        await LoadData(folderId);
    }
    [RelayCommand]
    public async Task LoadMoreItem(DataList.LoadMoreItemEventArgs args) {
        await DataController.PageDown(args.LastIndex, args.Amount);
    }
}
