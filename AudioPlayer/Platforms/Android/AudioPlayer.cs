using AudioCodec;

namespace AudioPlayer;

// All the code in this file is only included on Android.
#if ANDROID
using Android.Media;
using System.Diagnostics;

public partial class AudioPlayer {
    public readonly AudioCodec.AudioFormat Format;

    private readonly AudioTrack Player;
    private readonly AudioDecoder Decoder;
    private readonly Thread Worker;
    private readonly byte[] TransferBuffer;
    private readonly int AudioTrackBufferSize = 8 * 1024; // Around 100ms
    private readonly ManualResetEventSlim CanWriteEvent;
    private float _volume = 1.0f;
    private bool _isPlayed = false;
    private readonly CancellationTokenSource _disposeCts;
    public AudioPlayer() {
        this._disposeCts = new();
        this.Format = new(64 * 1024, 44_100, 2, AudioCodec.Enum.AVSampleFormat.S16);
        this.CanWriteEvent = new();
        int minBuffer = AudioTrack.GetMinBufferSize(
            Format.SampleRate,
            ChannelOut.Stereo,
            Encoding.Pcm16bit);
        var attributes = new AudioAttributes.Builder()
            .SetUsage(AudioUsageKind.Media)!
            .SetContentType(AudioContentType.Music)!
            .Build();
        var audioFormat = new AudioFormat.Builder()
            .SetSampleRate(Format.SampleRate)!
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
            Format.SampleRate,
            ChannelOut.Stereo,
            Encoding.Pcm16bit,
            Math.Max(minBuffer, Format.IoBufferSize),
            AudioTrackMode.Stream);
#endif
        this.AudioTrackBufferSize = this.Player.BufferSizeInFrames;
        this.TransferBuffer = new byte[8 * 1024];
        this.Decoder = new AudioDecoder(SeekCompleted, Format, 1 * 1024 * 1024);
        this.Worker = new Thread(WorkerLoop);
        this.Player.SetVolume(_volume);
        this.Worker.Start();
    }
    private void SeekCompleted() {
        this.Player.Flush();
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
                this.Player.Write(TransferBuffer, 0, readLength, WriteMode.Blocking);
            }
            else {
                try {
                    this.Decoder.Buffer.DataAvailable.Wait(this._disposeCts.Token);
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
        var metadata = this.Decoder.Metadata;
        if (metadata != null) {
            return metadata.Value.Duration;
        }
        else {
            return TimeSpan.Zero;
        }
    }
    [Obsolete]
    private TimeSpan GetTotalPosition() {
        int frames = this.Player.PlaybackHeadPosition;
        TimeSpan position = TimeSpan.FromSeconds((double)frames / Format.SampleRate);
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
        if (this.Worker.Join(Config.JoinTimeOut)) {
            this.Worker.Interrupt();
            this.Worker.Join();
        }
        this._disposeCts.Dispose();
        this.CanWriteEvent.Dispose();
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
