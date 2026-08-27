using MusicEco.Core.Services;
using MusicEco.Views.Shell;
using System.Diagnostics;

namespace MusicEco.Services;

public static partial class AppLifeCycle {
    private class Handler {
        private readonly Action<IServiceProvider>? _syncHandler;
        private readonly Func<IServiceProvider, Task>? _asyncHandler;
        public Handler(Action<IServiceProvider> handler) {
            this._syncHandler = handler;
            this._asyncHandler = null;
        }
        public Handler(Func<IServiceProvider, Task> handler) {
            this._syncHandler = null;
            this._asyncHandler = handler;
        }
        public async Task Invoke(IServiceProvider provider) {
            _syncHandler?.Invoke(provider);
            if (_asyncHandler != null) {
                await _asyncHandler.Invoke(provider);
            }
        }
    }
    private static readonly List<Action<IServiceProvider>> _closeHandlers = [];
    private static readonly List<Handler> _startHandlers = [];
    private static readonly List<Handler> _uiHandlers = [];
    private static bool _closed = false;
    private static bool _started = false;
    private static bool _ui = false;
    public static bool Closed => _closed;
    public static IServiceProvider Provider => App.Provider;
    public static async Task StartApp() {
        if (_started) {
            Debug.WriteLine("Duplicated start");
            return;
        }
        Debug.WriteLine("!!!---App starting---!!!");
        _started = true;
        Stopwatch sw = new();
        sw.Start();
        foreach (var handler in _startHandlers) {
            await handler.Invoke(Provider);
        }
        sw.Stop();
        _startHandlers.Clear();
        Debug.WriteLine($"!!!---App started---!!! {sw.ElapsedMilliseconds} ms");
    }
    public static async Task AfterUILoaded() {
        if (_ui) {
            Debug.WriteLine("Duplicated ui");
            return;
        }
        Debug.WriteLine("!!!---UI starting---!!!");
        _ui = true;
        Stopwatch sw = new();
        sw.Start();
        foreach (var handler in _uiHandlers) {
            await handler.Invoke(Provider);
        }
        sw.Stop();
        _uiHandlers.Clear();
        Debug.WriteLine($"!!!---UI started---!!! {sw.ElapsedMilliseconds} ms");
        WorkerLoop(Config.AppLoopDelta);
    }
    public static void CloseApp() {
        if (_closed) {
            Debug.WriteLine("Duplicated exit");
            return;
        }
        Debug.WriteLine("!!!---App closing---!!!");
        _closed = true;
        Stopwatch sw = new();
        sw.Start();
        foreach (var handler in _closeHandlers) {
            handler.Invoke(Provider);
        }
        sw.Stop();
        _closeHandlers.Clear();
        Debug.WriteLine($"!!!---App closed---!!! {sw.ElapsedMilliseconds} ms");
    }
    public static void RegisterAppClose(Action<IServiceProvider> action) {
        _closeHandlers.Add(new(action));
    }
    public static void RegisterAppStart(Action<IServiceProvider> action) {
        _startHandlers.Add(new(action));    
    }
    public static void RegisterAppStart(Func<IServiceProvider, Task> action) {
        _startHandlers.Add(new(action));
    }
    public static void RegisterAfterUILoaded(Action<IServiceProvider> action) {
        _uiHandlers.Add(new(action));
    }
    public static void RegisterAfterAppUI(Func<IServiceProvider, Task> action) {
        _uiHandlers.Add(new(action));
    }
}
