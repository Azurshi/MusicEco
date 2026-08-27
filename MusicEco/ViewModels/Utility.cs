namespace MusicEco.ViewModels;

public sealed class DelayedDispatcherEx {
    private readonly TimeSpan _delay;
    private CancellationTokenSource? _pending;
    public DelayedDispatcherEx(TimeSpan delay) {
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
public sealed partial class DelayedDispatcher: IDisposable {
    private readonly IDispatcherTimer _timer;
    private Action? _action;
    public DelayedDispatcher(IDispatcher dispatcher, TimeSpan delay) {
        this._timer = dispatcher.CreateTimer();
        this._timer.Interval = delay;
        this._timer.IsRepeating = false;
        this._timer.Tick += this.OnTick;
    }
    public void Dispatch(Action action) {
        this._action = action;
        this._timer.Stop();
        this._timer.Start();
    }
    private void OnTick(object? sender, EventArgs e) {
        var action = this._action;
        this._action = null;
        action?.Invoke();
    }
    public void Dispose() {
        this._timer.Stop();
        this._timer.Tick -= OnTick;
    }
}