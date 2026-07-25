namespace AudioCodec.Utility;

public sealed class SeekControl {
    private const long NoSeek = long.MinValue;
    private long _requestedSeekTicks = NoSeek;
    public ManualResetEvent HaveJobEvent = new(false);
    public void Seek(TimeSpan position) {
        ArgumentOutOfRangeException.ThrowIfLessThan(position, TimeSpan.Zero);
        Interlocked.Exchange(ref _requestedSeekTicks, position.Ticks);
        HaveJobEvent.Set();
    }
    public bool TryTakeSeek(out TimeSpan position) {
        long ticks = Interlocked.Exchange(ref _requestedSeekTicks, NoSeek);
        if (ticks == NoSeek) {
            position = default;
            return false;
        }
        position = TimeSpan.FromTicks(ticks);
        HaveJobEvent.Reset();
        return true;
    }
}

public sealed class StreamControl {
    private Stream? _requestStream = null;
    public ManualResetEvent HaveJobEvent = new(false);
    public void SetJob(Stream requestStream) {
        Interlocked.Exchange(ref _requestStream, requestStream);
        HaveJobEvent.Set();
    }
    public bool TryGetJob(out Stream? requestStream) {
        var job = Interlocked.Exchange(ref _requestStream, null);
        if (job == null) {
            requestStream = null;
            return false;
        }
        requestStream = job;
        HaveJobEvent.Reset();
        return true;
    }
    public bool HaveJob() {
        return _requestStream != null;
    }
}