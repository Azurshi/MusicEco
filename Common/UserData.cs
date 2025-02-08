using MusicEco.Global;
using MusicEco.Models.Base;

namespace MusicEco.Common;
public static class UserData {
    private const string Prefix = nameof(UserData);
    public static int CurrentSelectedPlaylist {
        get => GlobalData.GetValueOrDefault(Prefix + nameof(CurrentSelectedPlaylist), -1);
        set => GlobalData.Set(Prefix + nameof(CurrentSelectedPlaylist), value);
    }
    public static int CurrentSelectedQueue {
        get => GlobalData.GetValueOrDefault(Prefix + nameof(CurrentSelectedQueue), -1);
        set => GlobalData.Set(Prefix + nameof(CurrentSelectedQueue), value);
    }
    public static int CurrentPlayingQueue {
        get => MusicPlayer.LastPlayedQueue;
        set => GlobalData.Set(nameof(MusicPlayer) + nameof(MusicPlayer.LastPlayedQueue), value);
    }
}