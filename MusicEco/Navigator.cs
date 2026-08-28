namespace MusicEco;

public record PageRoute(string Route) {
    public static readonly PageRoute None = new(string.Empty);
    public static readonly PageRoute Home = new("home");
    public static readonly PageRoute Queue = new("queue");
    public static readonly PageRoute QueueDetail = new("queue/detail");
    public static readonly PageRoute Album = new("album");
    public static readonly PageRoute AlbumDetail = new("album/detail");
    public static readonly PageRoute Explorer = new("explorer");
    public static readonly PageRoute ExplorerTree = new("explorer/tree");
    public static readonly PageRoute Search = new("search");
    public static readonly PageRoute User = new("user");
    public static readonly PageRoute Setting = new("setting");

    public static readonly PageRoute Playlist = new("user/playlist");
    public static readonly PageRoute PlaylistDetail = new("user/playlist/detail");
    public static readonly PageRoute Favourite = new("user/favourite");
    public static readonly PageRoute PlayCount = new("user/playCount");
    public static readonly PageRoute PlayHistory = new("user/playHistory");
    public static readonly PageRoute NotPlay = new("user/notplay");
    public static readonly PageRoute AllSong = new("user/all");

    public static readonly PageRoute LanguageSetting = new("setting/language");
    public static readonly PageRoute InterfaceSetting = new("setting/interface");
    public static readonly PageRoute BackupSetting = new("setting/backup");
}

public class NavigateEventArgs: EventArgs {
    public PageRoute FromPage { get; }
    public PageRoute ToPage { get; }
    public Dictionary<string, object> Query { get; }
    public object? Sender;
    public NavigateEventArgs(object? sender, PageRoute fromPage, PageRoute toPage) {
        this.Sender = sender;
        this.FromPage = fromPage;
        this.ToPage = toPage;
        this.Query = [];
    }
    public NavigateEventArgs(object? sender, PageRoute frompage, PageRoute toPage, Dictionary<string, object> query) {
        this.Sender = sender;
        this.FromPage = frompage;
        this.ToPage = toPage;
        this.Query = query;
    }
}
public class NavigatedEventArgs: EventArgs {
    public PageRoute FromPage { get; }
    public PageRoute ToPage { get; }
    public Dictionary<string, object> Query { get; }
    public object? NavigateSender;
    public NavigatedEventArgs(object? navigateSender, PageRoute fromPage, PageRoute toPage) {
        this.NavigateSender = navigateSender;
        this.FromPage = fromPage;
        this.ToPage = toPage;
        this.Query = [];
    }
    public NavigatedEventArgs(object? navigateSender, PageRoute frompage, PageRoute toPage, Dictionary<string, object> query) {
        this.NavigateSender = navigateSender;
        this.FromPage = frompage;
        this.ToPage = toPage;
        this.Query = query;
    }
    public NavigatedEventArgs(NavigateEventArgs e) {
        this.NavigateSender = e.Sender;
        this.FromPage = e.FromPage;
        this.ToPage = e.ToPage;
        this.Query = e.Query;
    }
}
public class RefreshEventArgs: EventArgs {
}

