using AudioCodec.Enum;
using AudioCodec.Utility;
using System.Diagnostics;

namespace AudioCodec;

public sealed partial class AudioDecoder: IDisposable {
    private readonly DecoderConfig Config;
    private readonly SeekControl _seekControl;
    private readonly StreamControl _streamControl;
    private readonly AudioRingBuffer _ringBuffer;
    private readonly CancellationTokenSource _disposeCts;
    private CancellationTokenSource? _cts;
    public readonly ManualResetEvent ResetRequested;
    public readonly ManualResetEvent ResetCompleted;
    public event EventHandler? DecodeEnd;
    public long DecodedTicks = 0;
    public long DurationTicks = 0;
    public long CurrentJobId;
    public long BufferedTicks {
        get {
            long bufferLength = this._ringBuffer.LengthUpper;
            return bufferLength * TimeSpan.TicksPerSecond / Config.BytesPerSecond;
        }
    }
    private readonly ManualResetEvent _disposeEvent;
    private int _disposed;
    private readonly Thread _worker;
    public AudioRingBuffer Buffer => this._ringBuffer;
    public AudioDecoder(DecoderConfig config, int ringBufferCapacity) {
        this.Config = config;
        this.ResetRequested = new(false);
        this.ResetCompleted = new(false);
        this._disposeEvent = new(false);
        this._disposed = 0;
        this._ringBuffer = new(ringBufferCapacity);
        this._disposeCts = new();
        this._seekControl = new();
        this._streamControl = new();
        this._worker = new(this.WorkerLoop) {
            Name = nameof(AudioDecoder)
        };
        this.CurrentJobId = -1;
        this._worker.Start();
    }
    private void ConsumePacket(ReadOnlySpan<byte> data, long pts) {
        Volatile.Write(ref DecodedTicks, pts);
        var remaining = data;
        while (!remaining.IsEmpty) {
            int written = this._ringBuffer.Write(remaining);
            if (written == 0) {
                // Bufer is full
                // Wait when buffer is not full
                try {
                    int index = WaitHandle.WaitAny([
                        this._ringBuffer.CanWrite,
                        this._disposeEvent
                        ]);
                    if (index == 0) {
                        continue;
                    }
                    else if (index == 1) {
                        return;
                    }
                }
                catch (OperationCanceledException) {
                    return;
                }
                catch (ThreadInterruptedException) {
                    Debug.WriteLine("Force exit");
                    return;
                }
            }
            else {
                remaining = remaining.Slice(written);
            }
        }
    }
    public void Seek(TimeSpan time) {
        var duration = TimeSpan.FromTicks(Volatile.Read(ref this.DurationTicks));
        if (time < TimeSpan.Zero) {
            time = TimeSpan.Zero;
        }
        if (time > duration) {
            time = duration;
        }
        this._seekControl.Seek(time);
    }
    public void SetStream(Stream stream) {
        this._streamControl.SetJob(stream);
    }
    public TimeSpan GetDecodedDuration() {
        return TimeSpan.FromTicks(Volatile.Read(ref DecodedTicks));
    }
    public TimeSpan GetPlayingPosition() {
        long currentTicks = Volatile.Read(ref DecodedTicks) - BufferedTicks;
        return TimeSpan.FromTicks(currentTicks);
    }
    public TimeSpan GetPlayingPosition(long outerBufferedBytes) {
        long outerBufferedTicks = outerBufferedBytes * TimeSpan.TicksPerSecond / this.Config.BytesPerSecond;
        long currentTicks = Volatile.Read(ref DecodedTicks) - BufferedTicks - outerBufferedTicks;
        return TimeSpan.FromTicks(currentTicks);
    }

    public void Dispose() {
        if (Volatile.Read(ref this._disposed) > 0) {
            return;
        }
        Volatile.Write(ref this._disposed, 1);
        this._ringBuffer.Dispose();
        if (this._cts != null) {
            this._cts.Cancel();
            this._cts.Dispose();
        }
        this._disposeEvent.Set();
        this._disposeCts.Cancel();
        if (!this._worker.Join(AudioCodec.Config.JoinTimeOut)) {
            this._worker.Interrupt();
            this._worker.Join();
        }
        this._disposeCts.Dispose();
        this._disposeEvent.Dispose();
        this._seekControl.Dispose();
        this._streamControl.Dispose();
    }
}
