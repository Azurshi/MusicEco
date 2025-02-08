using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Common.Value;
using MusicEco.Global;
using MusicEco.Global.Attributes;
using MusicEco.Models;
using MusicEco.Models.Base;
using MusicEco.ViewModels.Base;
using MusicEco.ViewModels.Slots;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels.Sections;
public class ExplorerSection : PropertyObject {
    #region PersistanceData
    private static List<int> History {
        get => GlobalData.GetValueOrDefault(nameof(ExplorerSection) + nameof(History), new List<int>() { -1 });
        set => GlobalData.Set(nameof(ExplorerSection) + nameof(History), value);
    }
    private static int CurrentIt {
        get => GlobalData.GetValueOrDefault(nameof(ExplorerSection) + nameof(CurrentIt), 0);
        set => GlobalData.Set(nameof(ExplorerSection) + nameof(CurrentIt), value);
    }
    public static int CurrentId => History[CurrentIt];
    public static void Select(int folderId) {
        if (CurrentIt < History.Count -1) {
            History.RemoveRange(CurrentIt + 1, History.Count - (CurrentIt + 1));
        }
        List<int> localHistory = History;
        localHistory.Add(folderId);
        History = localHistory;
        CurrentIt = History.Count - 1;
    }
    public static void Backward() {
        if (CurrentIt > 1) {
            CurrentIt--;
        }
    }
    public static void Forward() {
        if (CurrentIt < History.Count -1) {
            CurrentIt++;
        }
    }
    public static void Up() {
        FolderModel? folderModel = FolderModel.Get(CurrentId);
        if (folderModel != null && folderModel.Parent != null) {
            Select(folderModel.Parent.Id);
        }
    }
    [StaticInitializer(-1)]
    public static void Clear() {
        History = [History[^1]];
        CurrentIt = 0;
    }
    #endregion
    public ExplorerSection() {
        DataController = new([], Setting.ListFrameAmount);
        EventSystem.Connect(Signal.System_Data_Loaded, OnDataLoaded);
    }
    private async void OnDataLoaded(object? sender, EventArgs e) {
        await UpdateData();
    }
    public readonly ObservableCollectionController<ExplorerSlot> DataController;
    public ObservableCollection<BaseSlot> Data => DataController.Target;
    private int _lastSelectedFolder = 1;
    public string LastSelectedFolderName => FolderModel.Get(_lastSelectedFolder)?.Name ?? "Unknow";
    #region DataModify
    public async Task UpdateData() {
        FolderModel? folderModel = FolderModel.Get(_lastSelectedFolder);
        if (folderModel != null) {
            List<int> childFolderIds = folderModel.GetChildFolder().Select(o => o.Id).ToList();
            List<int> childFileIds = folderModel.GetChildFile().Select(o => o.Id).ToList();
            List<string> totalKeys = [];
            foreach (var id in childFolderIds) {
                totalKeys.Add($"{Common.Value.Data.Item_FolderType}_{id}");
            }
            foreach (var id in childFileIds) {
                totalKeys.Add($"{Common.Value.Data.Item_FileType}_{id}");
            }
            await DataController.UpdateKeysAsync(totalKeys);
        } else {
            await DataController.UpdateKeysAsync([]);
        }
    }
    public void SelectFolder(int folderId) {
        _lastSelectedFolder = folderId;
    }
    public List<int> ExtractSongIds() {
        List<int> songIds = [];
        foreach(var slotData in Data) {
            string[] compactKey = slotData.Key.Split("_");
            if (compactKey.Length == 2) {
                string itemType = compactKey[0];
                int fileId = int.Parse(compactKey[1]);
                if (itemType == Common.Value.Data.Item_FileType) {
                    SongModel? songModel = SongModel.GetByFileId(fileId);
                    if (songModel != null) {
                        songIds.Add(songModel.Id);
                    }
                }
            }
        }
        return songIds;
    }
    #endregion
}