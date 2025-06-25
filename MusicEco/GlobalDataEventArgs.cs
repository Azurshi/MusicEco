namespace MusicEco;
public class PlayerProgressChangedEventArgs(float progress) : EventArgs {
    public float Progress { get; set; } = progress;
}
public class PlayerPlayingChangedEventArgs(bool isPlaying) : EventArgs {
    public bool IsPlaying { get; set; } = isPlaying;
}
public class PlayerIsShuffleChangedEventArgs(bool isShuffle) : EventArgs {
    public bool IsShuffle { get; set; } = isShuffle;
}
public class PlayerIsRepeatChangedEventArgs(bool isRepeat) : EventArgs {
    public bool IsRepeat { get; set; } = isRepeat;
}
public class PlayingQueueChangedEventArgs(long queueId) : EventArgs {
    public long QueueId { get; set; } = queueId;
}
public class PlayingSongChangedEventArgs(long songId) : EventArgs {
    public long SongId { get; set; } = songId;
}
public class PlayerVolumeChangedEventArgs(float percent) : EventArgs {
    public float Volume { get; set; } = percent;
}

public class CurrentFolderChangedEventArgs(long id, int index) : EventArgs {
    public long FolderId { get; set; } = id;
    public int Index { get; set; } = index;
}

public class ScanFoldersChangedEventArgs(List<string> oldFolders, List<string> newFolders): EventArgs {
    public List<string> OldFolders = oldFolders;
    public List<string> NewFolders = newFolders;
}

