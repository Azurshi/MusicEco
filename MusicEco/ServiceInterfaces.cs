using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco;

public interface IPageRouteRegistry {
    public Type GetPageType(PageRoute route);
}
public interface IPageResolver {
    public ContentView GetPage(PageRoute route);
}

public class CancelSource {
    public WeakReference<object> SenderReference { get; }
    public CancellationToken Token { get; }
    public CancelSource(object sender, CancellationToken token) {
        SenderReference = new(sender);
        Token = token;
    }
    public bool IsCancelled() {
        if (Token.IsCancellationRequested || !SenderReference.TryGetTarget(out var _)) {
            return true;
        }
        else {
            return false;
        }
    }
}

public interface IIconService {
    public Task InitializeDefault(IServiceProvider provider);
    public Task Setup(int nWorkers, int capacity);
    public Task<ImageSource> GetIcon(Hash256 hash, CoverSize size, CancelSource cancelSource);
}