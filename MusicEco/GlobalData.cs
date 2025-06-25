using Domain.DataAccess;
using Domain.EventSystem;
using Domain.Models;

namespace MusicEco;
public static class GlobalData {
    private static IModelGetter _modelGetter => IServiceAccess._modelGetter;
    private static DataStorage.DataAccess.GlobalData _globalData = new();

    public static float PlayerProgress {
        get => _globalData.GetValueOrDefault(nameof(PlayerProgress), 0f);
        set {
            _globalData.Set(nameof(PlayerProgress), value);
            EventSystem.Publish<PlayerProgressChangedEventArgs>(null, new(value));
        }
    }
    public static bool IsPlaying {
        get => _globalData.GetValueOrDefault(nameof(IsPlaying), false);
        set {
            _globalData.Set(nameof(IsPlaying), value);
            EventSystem.Publish<PlayerPlayingChangedEventArgs>(null, new(value));
        }
    }
    public static bool IsShuffle {
        get => _globalData.GetValueOrDefault(nameof(IsShuffle), false);
        set {
            _globalData.Set(nameof(IsShuffle), value);
            EventSystem.Publish<PlayerIsShuffleChangedEventArgs>(null, new(value));
        }
    }
    public static bool IsRepeat {
        get => _globalData.GetValueOrDefault(nameof(IsRepeat), false);
        set {
            _globalData.Set(nameof(IsRepeat), value);
            EventSystem.Publish<PlayerIsRepeatChangedEventArgs>(null, new(value));
        }
    }
    public static float PlayerVolume {
        get => _globalData.GetValueOrDefault(nameof(PlayerVolume), 1f);
        set {
            _globalData.Set(nameof(PlayerVolume), value);
            EventSystem.Publish<PlayerVolumeChangedEventArgs>(null, new(value));
        }
    }
    public static long PlayingQueueId {
        get => _globalData.GetValueOrDefault<long>(nameof(PlayingQueueId), 0);
        set {
            _globalData.Set(nameof(PlayingQueueId), value);
            EventSystem.Publish<PlayingQueueChangedEventArgs>(null, new(value));
        }
    }
    public static IPlaylistModel? CurrentQueue => _modelGetter.Playlist(PlayingQueueId);
    public static long PlayingSongId {
        get {
            IPlaylistModel? queueModel = CurrentQueue;
            if (queueModel != null) {
                return queueModel.Current?.Id ?? -1;
            }
            else {
                return -1;
            }
        }
    }
    public static ISongModel? CurrenSong => _modelGetter.Song(PlayingSongId);

    public static int CurrentFolderIndex {
        get => _globalData.GetValueOrDefault(nameof(CurrentFolderIndex), -1);
        private set {
            _globalData.Set(nameof(CurrentFolderIndex), value);
            long folderId = -1;
            if (value != -1) {
                folderId = FolderHistory[value];
            }
            EventSystem.Publish<CurrentFolderChangedEventArgs>(null, new(folderId, value));
        }
    }
    public static long? CurrentFolderId {
        get {
            if (CurrentFolderIndex != -1) {
                return FolderHistory[CurrentFolderIndex];
            }
            else {
                return null;
            }
        }
    }
    private static List<long> FolderHistory {
        get => _globalData.GetValueOrDefault(nameof(FolderHistory), new List<long>());
        set {
            _globalData.Set(nameof(FolderHistory), value);
        }
    }
    public static class Explorer {
        public static bool NavigateSelect(long id) {
            if (CurrentFolderIndex < FolderHistory.Count - 1) {
                FolderHistory.RemoveRange(CurrentFolderIndex + 1, FolderHistory.Count - (CurrentFolderIndex + 1));
            }
            List<long> localHistory = FolderHistory;
            localHistory.Add(id);
            FolderHistory = localHistory;
            CurrentFolderIndex = FolderHistory.Count - 1;
            return true;
        }
        public static long? NavigateBackward() {
            if (CurrentFolderIndex > 0) {
                CurrentFolderIndex--;
            }
            return CurrentFolderId;
        }
        public static long? NavigateForward() {
            if (CurrentFolderIndex < FolderHistory.Count - 1) {
                CurrentFolderIndex++;
            }
            return CurrentFolderId;
        }
        public static long NavigateClear() {
            CurrentFolderIndex = -1;
            FolderHistory = [];
            return -1;
        }
        public static long? NavigateRefresh() {
            return CurrentFolderIndex;
        }
        public static long? NavigateUp() {
            long? currentFolderId = CurrentFolderId;
            if (currentFolderId != null) {
                IFolderModel? folderModel = _modelGetter.Folder(currentFolderId ?? 0);
                if (folderModel != null && folderModel.Parent != null) {
                    NavigateSelect(folderModel.Parent.Id);
                    return folderModel.Parent.Id;
                }
            }
            return null;
        }
    }

    public static List<string> ScanFolders {
        get => _globalData.GetValueOrDefault(nameof(ScanFolders), new List<string>());
        set {
            List<string> oldFolders = _globalData.GetValueOrDefault(nameof(ScanFolders), new List<string>());
            _globalData.Set(nameof(ScanFolders), value);
            EventSystem.Publish<ScanFoldersChangedEventArgs>(null, new(oldFolders, value));
        }
    }
}
