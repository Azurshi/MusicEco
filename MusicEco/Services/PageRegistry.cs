using MusicEco.Views.Pages;
using MusicEco.Views.Pages.Settings;
using MusicEco.Views.Pages.Users;

namespace MusicEco.Services;

public class PageRegistry: IPageRouteRegistry {
    private Dictionary<string, Type> _pageTypes = [];
    public PageRegistry() {
        RegisterRoute<HomePage>(PageRoute.Home);
        RegisterRoute<QueuePage>(PageRoute.Queue);
        RegisterRoute<QueueDetailPage>(PageRoute.QueueDetail);
        RegisterRoute<AlbumPage>(PageRoute.Album);
        RegisterRoute<AlbumDetailPage>(PageRoute.AlbumDetail);
        RegisterRoute<ExplorerPage>(PageRoute.Explorer);
        RegisterRoute<ExplorerTreePage>(PageRoute.ExplorerTree);
        RegisterRoute<SearchPage>(PageRoute.Search);
        RegisterRoute<UserPage>(PageRoute.User);
        RegisterRoute<SettingPage>(PageRoute.Setting);

        // User
        RegisterRoute<PlaylistPage>(PageRoute.Playlist);
        RegisterRoute<PlaylistDetailPage>(PageRoute.PlaylistDetail);
        RegisterRoute<FavouritePage>(PageRoute.Favourite);
        RegisterRoute<PlayCountPage>(PageRoute.PlayCount);

        // Settings
        RegisterRoute<LanguageSettingPage>(PageRoute.LanguageSetting);
    }
    private void RegisterRoute<T>(PageRoute pageRoute) where T: ContentView {
        this._pageTypes[pageRoute.Route] = typeof(T);
    }
    public Type GetPageType(PageRoute route) {
        return this._pageTypes[route.Route];
    }
}