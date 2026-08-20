using Microsoft.Extensions.DependencyInjection;

namespace MusicEco.Core;

public interface IPlugin {
    public int Priority { get; }
    public abstract void RegisterService(IServiceCollection services);
    public abstract Task OnAppStarted(IServiceProvider provider);
    public abstract Task OnAppStopped(IServiceProvider provider);
}
