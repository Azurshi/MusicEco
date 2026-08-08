using AudioCodec;
using System.Diagnostics;

namespace AudioPlayer;

// All the code in this file is only included on Windows.
#if WINDOWS
using NAudio.Wave;
public partial class AudioPlayer: IDisposable {
    public readonly AudioFormat Format;
    private readonly BufferedWaveProvider Provider;
    private readonly WaveOut Player;
    private readonly AudioDecoder Decoder;
    private readonly Thread Worker;
    private readonly byte[] TransferBuffer;
    private bool _isPlayed = false;
    private readonly CancellationTokenSource _disposeCts;
    public AudioPlayer() {
        this._disposeCts = new();
        this.Format = new(64 * 1024, 44_100, 2, AudioCodec.Enum.AVSampleFormat.S16);
        this.Provider = new(new(Format.SampleRate, Format.BitSize, Format.Channels)) {
            DiscardOnBufferOverflow = true
        };
        this.TransferBuffer = new byte[8 * 1024];
        // This capture thread context so we need to use FunctionCallback
        this.Player = new WaveOut(WaveCallbackInfo.FunctionCallback());
        this.Player.Init(this.Provider);
        this.Decoder = new(SeekCompleted, this.Format, 1 * 1024 * 1024);
        this.Worker = new(WorkerLoop);
        this.Worker.Start();
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
                Thread.Sleep(100);  
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
            } else {
                // Manual set when need to dispose instead of wait with timeout
                try {
                    this.Decoder.Buffer.DataAvailable.Wait(this._disposeCts.Token);
                }
                catch (OperationCanceledException) {
                    return;
                }
            }
        }
    }
    private void SeekCompleted() {
        this.Provider.ClearBuffer();
    }
    public partial void Play(Stream stream) {
        this._isPlayed = true;
        this.Provider.ClearBuffer();
        this.Decoder.SetStream(stream);
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
        var metadata = this.Decoder.Metadata;
        if (metadata != null) {
            return metadata.Value.Duration;
        } else {
            return TimeSpan.Zero;
        }
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
        this.Worker.Join();
        this._disposeCts.Dispose();
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
