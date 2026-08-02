namespace AudioCodec.Utility;

public sealed class AudioRingBuffer: IDisposable {
    private readonly byte[] _data;
    private readonly int _capacity;
    private long _readCursor;
    private long _writeCursor;
    private int _disposed;
    public readonly ManualResetEventSlim JustRead = new();

    public int Capacity => _capacity;
    // Return current Length with a gap between write and read acquire
    // Return length is higher or equal than this
    public int LengthLower {
        get {
            long write = Volatile.Read(ref _writeCursor);
            long read = Volatile.Read(ref _readCursor);
            return (int)(write - read);
        }
    }
    // Return current Length with a gap between read and write acquire
    // Return length is lower or equal than this
    public int LengthUpper {
        get {
            long read = Volatile.Read(ref _readCursor);
            long write = Volatile.Read(ref _writeCursor);
            return (int)(write - read);
        }
    }
    public long TotalWriteBytes => Volatile.Read(ref _writeCursor);
    public readonly ManualResetEventSlim DataAvailable;
    public AudioRingBuffer(int capacity) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        this._capacity = capacity;
        this._data = new byte[this._capacity];
        this._readCursor = 0;
        this._writeCursor = 0;
        this._disposed = 0;
        this.DataAvailable = new();
    }
    public void Flush() {
        ThrowIfDisposed();
        long write = Volatile.Read(ref _writeCursor);
        Volatile.Write(ref _readCursor, write);
    }
    public int Write(ReadOnlySpan<byte> source) {
        ThrowIfDisposed();
        long read = Volatile.Read(ref _readCursor);
        long write = _writeCursor;
        int free = _capacity - checked((int)(write - read));
        int totalWriteCount = Math.Min(source.Length, free);
        if (totalWriteCount == 0) {
            return 0;
        }
        int writeIndex = (int)(write % _capacity);
        int firstWriteCount = Math.Min(totalWriteCount, _capacity - writeIndex);
        source.Slice(0, firstWriteCount).CopyTo(_data.AsSpan(writeIndex, firstWriteCount));
        if (totalWriteCount > firstWriteCount) {
            int remaingBytes = totalWriteCount - firstWriteCount;
            source.Slice(firstWriteCount, remaingBytes).CopyTo(this._data.AsSpan(0, remaingBytes));
        }
        Volatile.Write(ref _writeCursor, write + totalWriteCount);
        if (free == _capacity) {
            DataAvailable.Set();
        }
        return totalWriteCount;
    }
    public int Read(Span<byte> destination) {
        ThrowIfDisposed();
        long write = Volatile.Read(ref _writeCursor);
        long read = _readCursor;
        int totalBytesAvailablle = checked((int)(write - read));
        int totalReadCount = Math.Min(destination.Length, totalBytesAvailablle);
        if (totalReadCount == 0) {
            return 0;
        }
        int readIndex = (int)(read % _capacity);
        int firstReadCount = Math.Min(totalReadCount, _capacity - readIndex);
        this._data.AsSpan(readIndex, firstReadCount).CopyTo(destination.Slice(0, firstReadCount));
        if (totalReadCount > firstReadCount) {
            int remainingBytes = totalReadCount - firstReadCount;
            this._data.AsSpan(0, remainingBytes).CopyTo(destination.Slice(firstReadCount, remainingBytes));
        }
        Volatile.Write(ref _readCursor, read + totalReadCount);
        if (totalBytesAvailablle == totalReadCount) {
            DataAvailable.Reset();
        }
        if (totalReadCount > 0) {
            JustRead.Set();
        }
        return totalReadCount;
    }
    private void ThrowIfDisposed() {
        if (Volatile.Read(ref _disposed) != 0) {
            throw new ObjectDisposedException(nameof(AudioRingBuffer));
        }
    }
    public void Dispose() {
        Interlocked.Exchange(ref _disposed, 1);
    }
}
