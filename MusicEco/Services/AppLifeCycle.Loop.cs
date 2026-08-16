using System.Diagnostics;

namespace MusicEco.Services;

public static partial class AppLifeCycle {
    private class LoopHandler {
        public readonly string Key;
        private readonly Action<IServiceProvider>? _syncHandler;
        private readonly Func<IServiceProvider, Task>? _asyncHandler;
        public TimeSpan Elapsed;
        public readonly TimeSpan Interval;
        public LoopHandler(string key, Action<IServiceProvider> handler, TimeSpan interval) {
            this.Key = key;
            this._syncHandler = handler;
            this._asyncHandler = null;
            this.Elapsed = TimeSpan.Zero;
            this.Interval = interval;
        }
        public LoopHandler(string key, Func<IServiceProvider, Task> handler, TimeSpan interval) {
            this.Key = key;
            this._syncHandler = null;
            this._asyncHandler = handler;
            this.Elapsed = TimeSpan.Zero;
            this.Interval = interval;
        }
        public async Task Invoke(IServiceProvider provider) {
            _syncHandler?.Invoke(provider);
            if (_asyncHandler != null) {
                await _asyncHandler.Invoke(provider);
            }
        }
    }
    private static readonly List<LoopHandler> _loopHandlers = [];
    public static async void WorkerLoop(TimeSpan interval) {
        // Low-precision worker loop
        var sw = Stopwatch.StartNew();
        TimeSpan epsilon = TimeSpan.FromMilliseconds(1);
        TimeSpan lastElapsed = TimeSpan.Zero;
        while (!Closed) {
            sw.Restart();
            // Start jobs
            var provider = Provider;
            foreach(var handler in _loopHandlers) {
                handler.Elapsed += lastElapsed;
                if (handler.Elapsed >= handler.Interval) {
                    handler.Elapsed = TimeSpan.Zero;
                    await handler.Invoke(provider);
                }
            }
            // End jobs
            var elapsed = sw.Elapsed;
            sw.Restart();
            TimeSpan waitTime = interval - elapsed;
            if (waitTime < epsilon) {
                waitTime = epsilon;
            }
            await Task.Delay(waitTime);
            lastElapsed = elapsed + sw.Elapsed;
        }
        Debug.WriteLine("App loop stopped");
    }
    private static bool ExistsLoop(string key) {
        foreach (var handler in _loopHandlers) {
            if (handler.Key == key) {
                return true;
            }
        }
        return false;
    }
    public static void RegisterLoop(string key, Action<IServiceProvider> action, TimeSpan interval) {
        if (ExistsLoop(key)) {
            Debug.WriteLine($"AppLifeCycle :Remove old loop: {key}");
            UnRegisterLoop(key);
        }
        _loopHandlers.Add(new(key, action, interval));
    }
    public static void RegisterLoop(string key, Func<IServiceProvider, Task> action, TimeSpan interval) {
        if (ExistsLoop(key)) {
            Debug.WriteLine($"AppLifeCycle :Remove old loop: {key}");
            UnRegisterLoop(key);
        }
        _loopHandlers.Add(new(key, action, interval));
    }
    public static bool UnRegisterLoop(string key) {
        int handlerIndex = -1;
        for(int i=0; i<_loopHandlers.Count; i++) {
            if (_loopHandlers[i].Key == key) {
                handlerIndex = i;
                break;
            }
        }
        if (handlerIndex >= 0) {
            _loopHandlers.RemoveAt(handlerIndex);
            return true;
        } else {
            return false;
        }
    }
}
