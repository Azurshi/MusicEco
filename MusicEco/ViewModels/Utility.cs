using System.Diagnostics;

namespace MusicEco.ViewModels;

public sealed class DelayedDispatcher {
    private readonly TimeSpan _delay;
    private CancellationTokenSource? _pending;
    public DelayedDispatcher(TimeSpan delay) {
        this._delay = delay;
    }
    public async Task Dispatch(Action action) {
        if (this._pending != null) {
            this._pending.Cancel();
            // Already dispose on finally
        }
        var current = new CancellationTokenSource();
        this._pending = current;
        try {
            await Task.Delay(this._delay, current.Token);
            action();
        }
        catch (OperationCanceledException) when (current.IsCancellationRequested) {
            // Skip
        }
        finally {
            if (ReferenceEquals(this._pending, current)) {
                this._pending = null;
            }
            current.Dispose();
        }
    }
}