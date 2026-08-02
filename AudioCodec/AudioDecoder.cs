using AudioCodec.Enum;
using AudioCodec.Managed;
using AudioCodec.Types;
using AudioCodec.Utility;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AudioCodec;

public readonly struct AudioFormat(int ioBufferSize, int sampleRate, int channels, AVSampleFormat sampleFormat) {
    public readonly int IoBufferSize = ioBufferSize;
    public readonly int SampleRate = sampleRate;
    public readonly int Channels = channels;
    public readonly AVSampleFormat SampleFormat = sampleFormat;
    public readonly int ByteSize => CodecManaged.GetBytesPerSample(SampleFormat);
    public readonly int BitSize => 8 * ByteSize;
    public readonly int BytesPerSecond => SampleRate * Channels * ByteSize;
    public readonly TimeSpan CalculateDurationFromBytesLength(long bytesLength) {
        return TimeSpan.FromTicks(bytesLength * TimeSpan.TicksPerSecond / BytesPerSecond);
    }
}
public delegate void BufferResetHandler();
public delegate void DecodeEndEventHandler();
public delegate void ConsumePacketHandler(PacketData data);
public sealed class AudioDecoder: IDisposable {
    public event DecodeEndEventHandler? DecodeEnd;
    private readonly SeekControl _seekControl;
    private readonly StreamControl _streamControl;
    private bool _disposed;
    private readonly CancellationTokenSource _disposeCts;
    private readonly ManualResetEvent _disposeEvent;
    private readonly Lock _metadataLock = new();
    private AudioMetadata? _metadata = null;
    public AudioMetadata? Metadata {
        get {
            lock (_metadataLock) {
                return _metadata;
            }
        }
    }
    public readonly AudioFormat Format;
    private readonly BufferResetHandler _bufferReset;
    private readonly ConsumePacketHandler _consumePacket;
    public bool Disposed => _disposed;
    private readonly Thread _worker;
    private readonly AudioRingBuffer _ringBuffer;
    public AudioRingBuffer Buffer => _ringBuffer;
    public long DecodedTicks = 0;
    public long BufferedTicks {
        get {
            long bufferLength = this._ringBuffer.LengthUpper;
            return bufferLength * TimeSpan.TicksPerSecond / Format.BytesPerSecond;
        }
    }
    // Called after 
    public AudioDecoder(
        BufferResetHandler bufferReset,
        AudioFormat format,
        int ringBufferCapacity) {
        this._disposeCts = new();
        this._disposeEvent = new(false);
        this._worker = new(WorkerLoop);
        this._ringBuffer = new(ringBufferCapacity);
        this._seekControl = new();
        this._streamControl = new();
        this._disposed = false;
        this._bufferReset = bufferReset;
        this.Format = format;
        this._consumePacket = new((pack) => {
            Volatile.Write(ref DecodedTicks, pack.TicksPTS);
            var remaining = pack.Data;
            while (!remaining.IsEmpty) {
                int written = this._ringBuffer.Write(remaining);
                if (written == 0) {
                    // Bufer is full
                    // Reset signal state
                    _ringBuffer.JustRead.Reset();
                    // Wait when buffer is not full
                    try {
                        _ringBuffer.JustRead.Wait(this._disposeCts.Token);
                    }
                    catch (OperationCanceledException){
                        return;
                    }
                }
                else {
                    remaining = remaining.Slice(written);
                }
            }
        });
        this._worker.Start();
    }
    private CancellationTokenSource? _cts;
    private void TryScheduleNewStream() {
        if (_streamControl.TryGetJob(out var stream)) {
            if (_cts != null) {
                _cts.Cancel();
                _cts.Dispose();
            }
            _cts = new();
            Debug.WriteLine("Schedule new job");
            var token = _cts.Token;
            DecodeToPCM(stream!, token);
        }
    }
    private void WorkerLoop() {
        while(!this._disposed) {
            // This code only invoked at entry, then lock into DecodeToPCM
            // Or run again when new stream is requested
            if (_streamControl.TryGetJob(out var stream)) {
                if (_cts != null) {
                    _cts.Cancel();
                    _cts.Dispose();
                }
                _cts = new();
                this._ringBuffer.Flush();
                this._bufferReset();
                Debug.WriteLine("Schedule new job");
                var token = _cts.Token; 
                DecodeToPCM(stream!, token); // This is what thread is running on
            } else {
                // This does not reach since thread is locked at DecodeToPCM
                // Only use when entry, to wait for start job
                int index = WaitHandle.WaitAny([_streamControl.HaveJobEvent, _disposeEvent]);
                if (index == 0) {
                    // Continue
                    continue;
                }
                else if (index == 1) {
                    // Dispose
                    return;
                }
                else {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    public void Dispose() {
        if (this._disposed) {
            return;
        }
        this._disposed = true;
        this._ringBuffer.Dispose();
        if (_cts != null) {
            _cts.Cancel();
            _cts.Dispose();
        }
        this._disposeEvent.Set();
        this._disposeCts.Cancel();
        this._worker.Join();
        this._disposeCts.Dispose();
        this._disposeEvent.Dispose();
        this._seekControl.Dispose();
        this._streamControl.Dispose();
    }
    private unsafe void DecodeToPCM(Stream input, CancellationToken token) {
        using (var managedFormat = new FormatFromStream(input, this.Format.IoBufferSize)) {
            AVFormatContext* format = managedFormat.Context;
            AVCodecContext* codecContext = null;
            AVPacket* packet = null;
            AVFrame* frame = null;
            SwrContext* resampler = null;
            try {
                int bytesPerSample = CodecManaged.GetBytesPerSample(this.Format.SampleFormat);
                int result = FFmpeg.Format.avformat_find_stream_info(format, null);
                CodecManaged.CheckResult(result);
                AudioMetadata metadata = new(
                    duration: TimeSpan.FromTicks(format->Duration*10)
                    );
                lock(_metadataLock) {
                    this._metadata = metadata;
                }
                int audioStreamIndex = CodecManaged.FindStreamIndex(format, AVMediaType.AUDIO);
                if (audioStreamIndex < 0) {
                    throw new InvalidOleVariantTypeException("No audio stream found");
                }

                AVStream* stream = format->Streams[audioStreamIndex];
                AVCodecParameters* parameters = stream->CodecParameters;
                AVCodec* decoder = FFmpeg.Codec.avcodec_find_decoder(parameters->CodecId);
                if (decoder == null) {
                    throw new InvalidOperationException("");
                }
                codecContext = FFmpeg.Codec.avcodec_alloc_context3(decoder);
                result = FFmpeg.Codec.avcodec_parameters_to_context(codecContext, parameters);
                CodecManaged.CheckResult(result);
                result = FFmpeg.Codec.avcodec_open2(codecContext, decoder, null);
                CodecManaged.CheckResult(result);
                AVChannelLayout outputLayout = default;
                FFmpeg.Util.av_channel_layout_default(&outputLayout, this.Format.Channels);
                result = FFmpeg.SWR.swr_alloc_set_opts2(
                    &resampler,
                    &outputLayout,
                    this.Format.SampleFormat,
                    this.Format.SampleRate,
                    &codecContext->ChannelLayout,
                    codecContext->SampleFormat,
                    codecContext->SampleRate,
                    0,
                    null);
                CodecManaged.CheckResult(result);
                result = FFmpeg.SWR.swr_init(resampler);
                CodecManaged.CheckResult(result);
                packet = FFmpeg.Codec.av_packet_alloc();
                frame = FFmpeg.Util.av_frame_alloc();
                AVRational timeBase = stream->TimeBase;
                while (!this._disposed) {
                    if (_streamControl.HaveJob()) {
                        _cts!.Cancel();
                    }
                    if (token.IsCancellationRequested) {
                        Debug.WriteLine("Decode cancelled");
                        break;
                    }
                    if (this._seekControl.TryTakeSeek(out TimeSpan target)) {
                        CodecManaged.SeekDecoder(
                            format,
                            codecContext,
                            resampler,
                            packet,
                            frame,
                            audioStreamIndex,
                            timeBase,
                            target);
                        this._ringBuffer.Flush();
                        this._bufferReset();
                    }
                    result = FFmpeg.Format.av_read_frame(format, packet);
                    if (result == FFmpeg.Flags.AVERR_EOF) {
                        DecodeEnd?.Invoke();
                        int index = WaitHandle.WaitAny([_streamControl.HaveJobEvent, _seekControl.HaveJobEvent, _disposeEvent]);
                        if (index == 0) {
                            // Move to if(_streamControl.HaveJob())
                            continue;
                        } 
                        else if (index == 1) {
                            // Move to next cycle
                            continue;
                        }
                        else if (index == 2) {
                            // Dispose
                            return;
                        }
                        else {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                    else {
                        if (packet->StreamIndex == audioStreamIndex) {
                            result = FFmpeg.Codec.avcodec_send_packet(
                                codecContext,
                                packet);
                            CodecManaged.CheckResult(result);
                            CodecManaged.ReceiveFrames(
                                codecContext,
                                resampler,
                                frame,
                                this._consumePacket.Invoke,
                                this.Format.Channels,
                                bytesPerSample,
                                timeBase);
                        }
                        FFmpeg.Codec.av_packet_unref(packet);
                    }
                }
                // Flush 
                _ = FFmpeg.Codec.avcodec_send_packet(codecContext, null);
                CodecManaged.ReceiveFrames(
                    codecContext,
                    resampler,
                    frame,
                    this._consumePacket.Invoke,
                    this.Format.Channels,
                    bytesPerSample,
                    timeBase);
                CodecManaged.FlushResampler(
                    resampler,
                    this._consumePacket.Invoke,
                    this.Format.Channels,
                    bytesPerSample,
                    frame->Pts,
                    timeBase);
            }
            finally {
                FFmpeg.SWR.swr_free(&resampler);
                FFmpeg.Util.av_frame_free(&frame);
                FFmpeg.Codec.av_packet_free(&packet);
                FFmpeg.Codec.avcodec_free_context(&codecContext);
                input.Close();
            }
        }
    }
    public void Seek(TimeSpan time) {
        var metadata = this.Metadata;
        if (metadata == null) {
            return;
        }
        if (time < TimeSpan.Zero) {
            time = TimeSpan.Zero;
        }
        if (time > metadata.Value.Duration) {
            time = metadata.Value.Duration;
        }
        this._seekControl.Seek(time);
    }
    public void SetStream(Stream stream) {
        _streamControl.SetJob(stream);
    }
    public TimeSpan GetDecodedDuration() {
        return TimeSpan.FromTicks(Volatile.Read(ref DecodedTicks));
    }
    public TimeSpan GetPlayingPosition() {
        long currentTicks = Volatile.Read(ref DecodedTicks) - BufferedTicks;
        return TimeSpan.FromTicks(currentTicks);
    }
    public TimeSpan GetPlayingPosition(long outerBufferedBytes) {
        long outerBufferedTicks = outerBufferedBytes * TimeSpan.TicksPerSecond / Format.BytesPerSecond;
        long currentTicks = Volatile.Read(ref DecodedTicks) - BufferedTicks - outerBufferedTicks;
        return TimeSpan.FromTicks(currentTicks);
    }

    public static Dictionary<string, string> ReadMetadata(Stream stream, int bufferSize = 8 * 1024) {
        using (var managedFormat = new FormatFromStream(stream, bufferSize)) {
            Dictionary<string, string> metadata = new(StringComparer.OrdinalIgnoreCase);
            unsafe {
                AVFormatContext* format = managedFormat.Context;
                AVDictionaryEntry* previous = null;
                while (true) {
                    AVDictionaryEntry* entry = FFmpeg.Util.av_dict_iterate(format->Metadata, previous);
                    if (entry == null) {
                        break;
                    }
                    string key = Marshal.PtrToStringUTF8((nint)entry->Key) ?? string.Empty;
                    string value = Marshal.PtrToStringUTF8((nint)entry->Value) ?? string.Empty;
                    previous = entry;
                    metadata[key] = value;
                }
            }
            return metadata;
        }
    }
}
