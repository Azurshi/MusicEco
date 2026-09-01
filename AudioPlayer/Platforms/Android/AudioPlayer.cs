namespace AudioPlayer;

// All the code in this file is only included on Android.
#if ANDROID
using Android.Media;
using AudioCodec;
using System.Diagnostics;

public partial class AudioPlayer {
    public readonly DecoderConfig Config;

    private readonly AudioTrack Player;
    private readonly AudioDecoder Decoder;
    private readonly Thread Worker;
    private readonly byte[] TransferBuffer;
    private readonly int AudioTrackBufferSize = 8 * 1024; // Around 100ms
    private readonly int PollMs = 10; // Must less than buffer duration
    private readonly ManualResetEventSlim CanWriteEvent;
    private float _volume = 1.0f;
    private bool _isPlayed = false;
    private readonly ManualResetEvent _disposeEvent;
    private readonly CancellationTokenSource _disposeCts;
    private int _firstPackRead = 0;
    public AudioPlayer() {
        this.Config = new(2, 44_100, AudioCodec.Enum.AVSampleFormat.S16, 64 * 1024);
        this._disposeCts = new();
        this.CanWriteEvent = new();
        this._disposeEvent = new(false);
        int minBuffer = AudioTrack.GetMinBufferSize(
            this.Config.OutputSampleRate,
            ChannelOut.Stereo,
            Encoding.Pcm16bit);
        var attributes = new AudioAttributes.Builder()
            .SetUsage(AudioUsageKind.Media)!
            .SetContentType(AudioContentType.Music)!
            .Build();
        var audioFormat = new Android.Media.AudioFormat.Builder()
            .SetSampleRate(this.Config.OutputSampleRate)!
            .SetEncoding(Encoding.Pcm16bit)!
            .SetChannelMask(ChannelOut.Stereo)
            .Build();
#if ANDROID23_0_OR_GREATER
        this.Player = new AudioTrack.Builder()
            .SetAudioAttributes(attributes!)
            .SetAudioFormat(audioFormat!)
            .SetBufferSizeInBytes(AudioTrackBufferSize)
            .Build();
#else
        this.Player = new AudioTrack(
            Stream.Music,
            this.Config.OutputSampleRate,
            ChannelOut.Stereo,
            Encoding.Pcm16bit,
            Math.Max(minBuffer, this.Config.IOBufferSize),
            AudioTrackMode.Stream);
#endif
        this.AudioTrackBufferSize = this.Player.BufferSizeInFrames;
        this.TransferBuffer = new byte[8 * 1024];
        this.Decoder = new AudioDecoder(this.Config, 1 * 1024 * 1024);
        this.Worker = new Thread(WorkerLoop);
        this.Player.SetVolume(_volume);
        this.Worker.Start();
    }
    private void ResetSynchronize() {
        Debug.WriteLine("Player: Reset synchronize started");
        this.Player.Flush();
        this.Decoder.Buffer.Flush();
        this.Decoder.ResetCompleted.Set();
        this.Decoder.ResetRequested.Reset();
        Volatile.Write(ref this._firstPackRead, 0);
        Debug.WriteLine("Player: Reset synchronize completed");
    }
    private void WorkerLoop() {
        while (true) {
            int readLength;
            try {
                readLength = this.Decoder.Buffer.Read(TransferBuffer);
            }
            catch (ObjectDisposedException) {
                break;
            }
            if (readLength > 0) {
                try {
                    CanWriteEvent.Wait(this._disposeCts.Token);
                }
                catch (OperationCanceledException) {
                    return;
                }
                catch (ThreadInterruptedException) {
                    Debug.WriteLine("Force exit");
                    return;
                }
                int offset = 0;
                int remaining = readLength;
                while(remaining > 0) {
                    int written = this.Player.Write(TransferBuffer, offset, remaining, WriteMode.NonBlocking);
                    if (written > 0) {
                        // This will work unless Player starve it's buffer before decoder can fill it
                        if (Volatile.Read(ref this._firstPackRead) <= 0) {
                            Volatile.Write(ref this._firstPackRead, 1);
                        }
                        offset += written;
                        remaining -= written;
                    }
                    else {
                        if (this.Decoder.ResetRequested.WaitOne(TimeSpan.FromMilliseconds(this.PollMs))) {
                            Debug.WriteLine("Player: wait Player to write empty then reset");
                            ResetSynchronize();
                        }
                    }
                    if (this._disposeCts.IsCancellationRequested) {
                        return;
                    }
                }
            }
            else {
                try {
                    int index = WaitHandle.WaitAny([
                        this.Decoder.Buffer.DataAvailable,
                        this.Decoder.ResetRequested,
                        this._disposeEvent
                        ]);
                    if (index == 0) {
                        continue;
                    }
                    else if (index == 1) {
                        this.ResetSynchronize();
                    }
                    else if (index == 2) {
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
        }
    }
    public partial void Play(System.IO.Stream stream) {
        this._isPlayed = true;
        this.CanWriteEvent.Set();
        this.Player.Flush();
        this.Decoder.SetStream(stream);
        Volatile.Write(ref this._firstPackRead, 0); // Guardd
        this.Player.Play();
    }
    public partial void Seek(TimeSpan position) {
        this.Decoder.Seek(position);
    }
    public partial void Pause() {
        this._isPlayed = false;
        this.CanWriteEvent.Reset();
        this.Player.Stop();
    }
    public partial void Resume() {
        this._isPlayed = true;
        this.CanWriteEvent.Set();
        this.Player.Play();
    }
    public partial TimeSpan GetDuration() {
        return TimeSpan.FromTicks(Volatile.Read(ref this.Decoder.DurationTicks));
    }
    [Obsolete]
    private TimeSpan GetTotalPosition() {
        int frames = this.Player.PlaybackHeadPosition;
        TimeSpan position = TimeSpan.FromSeconds((double)frames / this.Config.OutputSampleRate);
        return position;
    }
    public partial TimeSpan GetPosition() {
        // Estimated only, around +- 25ms
        var position = this.Decoder.GetPlayingPosition(this.AudioTrackBufferSize / 2);
        return this.ClampPosition(position);
    }
    public partial TimeSpan GetDecodedPosition() {
        return this.Decoder.GetDecodedDuration();
    }
    public partial void Dispose() {
        this.Player.Stop();
        this.Player.Release();
        this.Player.Dispose();
        this._disposeCts.Cancel();
        this._disposeEvent.Set();
        if (this.Worker.Join(LocalConfig.JoinTimeOut)) {
            this.Worker.Interrupt();
            this.Worker.Join();
        }
        this._disposeCts.Dispose();
        this.CanWriteEvent.Dispose();
        this._disposeEvent.Dispose();
    }
    public partial float GetVolume() {
        return this.ClampVolume(this._volume);
    }
    public partial void SetVolume(float volume) {
        volume = this.ClampVolume(volume);
        this._volume = volume;
        this.Player.SetVolume(volume);
    }
    /// <summary>
    /// May deviate by 100ms
    /// </summary>
    /// <returns></returns>
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
                && (
                    this.GetDuration() <= this.CheckEndEpsilon 
                    || this.GetDuration() - this.GetPosition() < this.CheckEndEpsilon
                    )
                ) {
                return PlaybackState.End;
            }
            else {
                return PlaybackState.Playing;
            }
        }
        else {
            return PlaybackState.Paused;
        }
    }
}
#endif
