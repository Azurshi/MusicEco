using AudioCodec;
using System.Diagnostics;

namespace AudioPlayer;

using AudioCodec.Enum;

// All the code in this file is only included on Windows.
#if WINDOWS
using NAudio.Wave;
public partial class AudioPlayer: IDisposable {
    private readonly BufferedWaveProvider Provider;
    private readonly WaveOut Player;
    private readonly AudioDecoder Decoder;
    private readonly Thread Worker;
    private readonly byte[] TransferBuffer;
    private bool _isPlayed = false;
    private readonly CancellationTokenSource _disposeCts;
    private readonly ManualResetEvent _disposeEvent;
    private int _firstPackRead = 0;
    public AudioPlayer() {
        this._disposeCts = new();
        DecoderConfig config = new(2, 44_100, AVSampleFormat.S16, 64 * 1024);
        this.Provider = new(new(config.OutputSampleRate, config.BitsPerSample, config.OutputChannels)) {
            DiscardOnBufferOverflow = true
        };
        this.TransferBuffer = new byte[8 * 1024];
        // This capture thread context so we need to use FunctionCallback
        this.Player = new WaveOut(WaveCallbackInfo.FunctionCallback());
        this.Player.Init(this.Provider);
        this.Decoder = new(config, 1 * 1024 * 1024);
        this._disposeEvent = new(false);
        this.Worker = new(WorkerLoop) {
            Name = nameof(AudioPlayer)
        };
        this.Worker.Start();
    }
    private void ResetSynchronize() {
        Debug.WriteLine("Player: Reset synchronize started");
        this.Provider.ClearBuffer();
        this.Decoder.Buffer.Flush();
        this.Decoder.ResetCompleted.Set();
        this.Decoder.ResetRequested.Reset();
        Volatile.Write(ref this._firstPackRead, 0);
        Debug.WriteLine("Player: Reset synchronize completed");
    }
    private void WorkerLoop() {
        int providerBufferLength = this.Provider.BufferLength;
        int pad = 1024;
        while(true) {
            int bufferedBytes = this.Provider.BufferedBytes;
            int freeSpace = providerBufferLength - bufferedBytes - pad;
            if (bufferedBytes < 1024) {
                Debug.WriteLine($"WARNING: Buffered length: {bufferedBytes}");
            }
            while(freeSpace < TransferBuffer.Length) {
                if (this.Decoder.ResetRequested.WaitOne(TimeSpan.FromMilliseconds(100))) {
                    Debug.WriteLine("Player: wait Provider to consumer then reset");
                    ResetSynchronize();
                }
                if (this._disposeCts.IsCancellationRequested) {
                    return;
                }
                freeSpace = providerBufferLength - this.Provider.BufferedBytes - pad;
            }
            int readLength;
            try {
                readLength = this.Decoder.Buffer.Read(TransferBuffer.AsSpan());
            }
            catch (ObjectDisposedException) {
                break;
            }
            if (readLength > 0) {
                Provider.AddSamples(TransferBuffer, 0, readLength);
                // This will work unless Provider starve it's buffer before decoder can fill it
                if (Volatile.Read(ref this._firstPackRead) <= 0) {
                    Volatile.Write(ref this._firstPackRead, 1);
                }
            } else {
                try {
                    int index = WaitHandle.WaitAny([
                        this.Decoder.Buffer.DataAvailable,
                        this.Decoder.ResetRequested,
                        this._disposeEvent
                        ]);
                    if (index == 0) {
                        // When switch to new Audio or seek, it need to ramp up to fill buffer
                        // So repeated reach this during new Audio or seek is not an error
                        //Debug.WriteLine("Player: read 0 then Data available");
                        continue;
                    }
                    else if (index == 1) {
                        Debug.WriteLine("Player: read 0 then Reset");
                        ResetSynchronize();
                    }
                    else if (index == 2) {
                        return;
                    }
                    else {
                        throw new ArgumentOutOfRangeException();
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
        }
    }
    public partial void Play(Stream stream) {
        this._isPlayed = true;
        this.Provider.ClearBuffer();
        this.Decoder.SetStream(stream);
        Volatile.Write(ref this._firstPackRead, 0); // Guardd
        this.Player.Play();
    }
    public partial void Seek(TimeSpan position) {
        this.Decoder.Seek(position);
    }
    public partial void Pause() {
        this._isPlayed = false;
        this.Player.Stop();
    }
    public partial void Resume() {
        this._isPlayed = true;
        this.Player.Play();
    }
    public partial TimeSpan GetDuration() {
        return TimeSpan.FromTicks(Volatile.Read(ref this.Decoder.DurationTicks));
    }
    public partial TimeSpan GetPosition() {
        var position = this.Decoder.GetPlayingPosition(this.Provider.BufferedBytes);
        return this.ClampPosition(position);
    }
    public partial TimeSpan GetDecodedPosition() {
        return this.Decoder.GetDecodedDuration();
    }
    public partial void Dispose() {
        this.Provider.ClearBuffer();
        this.Player.Dispose();
        this.Decoder.Dispose();
        this._disposeCts.Cancel();
        this._disposeEvent.Set();
        if (this.Worker.Join(LocalConfig.JoinTimeOut)) {
            this.Worker.Interrupt();
            this.Worker.Join();
        }
        this._disposeCts.Dispose();
        this._disposeEvent.Dispose();
    }
    public partial float GetVolume() {
        return this.ClampVolume(this.Player.Volume);
    }
    public partial void SetVolume(float volume) {
        volume = this.ClampVolume(volume);
        this.Player.Volume = volume;
    }
    public partial PlaybackState GetState() {
        if (this._isPlayed) {
            // Still loading
            if (Volatile.Read(ref this._firstPackRead) <= 0) {
                Debug.WriteLine("Player: Loading");
                return PlaybackState.Playing;
            }
            // Should work unless thread is blocked or CPU can't keep up
            // Add a guard by check position
            if (this.Decoder.Buffer.LengthUpper == 0 
                && this.Provider.BufferedBytes == 0
                && (
                    this.GetDuration() <= this.CheckEndEpsilon
                    || this.GetDuration() - this.GetPosition() < this.CheckEndEpsilon
                    )
                ) {
                return PlaybackState.End;
            } else {
                return PlaybackState.Playing;
            }
        } else {
            return PlaybackState.Paused;
        }
    }
}
#endif
