namespace MusicEco.Services;

public class PageResolver: IPageResolver {
    private readonly IServiceProvider _provider;
    private readonly IPageRouteRegistry _registry;
    public PageResolver(IServiceProvider provider, IPageRouteRegistry registry) {
        this._provider = provider;
        this._registry = registry;
    }
    public ContentView GetPage(PageRoute route) {
        object page = this._provider.GetRequiredService(this._registry.GetPageType(route));
        if (page is ContentView view) {
            return view;
        } else {
            throw new Exception($"Page is not ContentView {page.GetType()}");
        }
    }
}
