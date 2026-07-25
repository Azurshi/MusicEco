using AudioCodec.Enum;
using AudioCodec.Managed;
using AudioCodec.Types;
using AudioCodec.Utility;
using System.Runtime.InteropServices;

namespace AudioCodec;

public static partial class CodecManaged {
    private static readonly AVRational TimeSpanRational = new() { Numerator = 1, Denominator = (int)TimeSpan.TicksPerSecond };
    public static unsafe void DecodeToPCM(
        Stream input,
        SeekControl? control,
        Action<PacketData> consumer,
        Action<AudioMetadata> setData,
        Action seekCallback,
        int ioBufferSize,
        int outputSampleRate,
        int outputChannels,
        AVSampleFormat outputFormat
        ) {
        using (var managedFormat = new FormatFromStream(input, ioBufferSize)) {
            AVFormatContext* format = managedFormat.Context;
            AVCodecContext* codecContext = null;
            AVPacket* packet = null;
            AVFrame* frame = null;
            SwrContext* resampler = null;
            try {
                int bytesPerSample = GetBytesPerSample(outputFormat);
                int result = FFmpeg.Format.avformat_find_stream_info(format, null);
                CheckResult(result);
                AudioMetadata metadata = new(
                    duration: TimeSpan.FromTicks(format->Duration * 10)
                    );
                setData(metadata);
                int audioStreamIndex = FindStreamIndex(format, AVMediaType.AUDIO);
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
                CheckResult(result);
                result = FFmpeg.Codec.avcodec_open2(codecContext, decoder, null);
                CheckResult(result);
                AVChannelLayout outputLayout = default;
                FFmpeg.Util.av_channel_layout_default(&outputLayout, outputChannels);
                result = FFmpeg.SWR.swr_alloc_set_opts2(
                    &resampler,
                    &outputLayout,
                    outputFormat,
                    outputSampleRate,
                    &codecContext->ChannelLayout,
                    codecContext->SampleFormat,
                    codecContext->SampleRate,
                    0,
                    null);
                CheckResult(result);
                result = FFmpeg.SWR.swr_init(resampler);
                CheckResult(result);
                packet = FFmpeg.Codec.av_packet_alloc();
                frame = FFmpeg.Util.av_frame_alloc();
                AVRational timeBase = stream->TimeBase;
                while (true) {
                    if (control != null && control.TryTakeSeek(out TimeSpan target)) {
                        SeekDecoder(
                            format,
                            codecContext,
                            resampler,
                            packet,
                            frame,
                            audioStreamIndex,
                            timeBase,
                            target);
                        seekCallback();
                    }
                    result = FFmpeg.Format.av_read_frame(format, packet);
                    if (result == FFmpeg.Flags.AVERR_EOF) {
                        break;
                    }
                    if (packet->StreamIndex == audioStreamIndex) {
                        result = FFmpeg.Codec.avcodec_send_packet(
                            codecContext,
                            packet);
                        CheckResult(result);
                        ReceiveFrames(
                            codecContext,
                            resampler,
                            frame,
                            consumer,
                            outputChannels,
                            bytesPerSample,
                            timeBase);
                    }
                    FFmpeg.Codec.av_packet_unref(packet);
                }
                // Flush 
                _ = FFmpeg.Codec.avcodec_send_packet(codecContext, null);
                ReceiveFrames(
                    codecContext,
                    resampler,
                    frame,
                    consumer,
                    outputChannels,
                    bytesPerSample,
                    timeBase);
                FlushResampler(
                    resampler,
                    consumer,
                    outputChannels, 
                    bytesPerSample, 
                    frame->Pts,
                    timeBase);
            }
            finally {
                FFmpeg.SWR.swr_free(&resampler);
                FFmpeg.Util.av_frame_free(&frame);
                FFmpeg.Codec.av_packet_free(&packet);
                FFmpeg.Codec.avcodec_free_context(&codecContext);
            }
        }
    }
    internal static unsafe void SeekDecoder(
        AVFormatContext* format,
        AVCodecContext* codec,
        SwrContext* resampler,
        AVPacket* packet,
        AVFrame* frame,
        int audioStreamIndex,
        AVRational timeBase,
        TimeSpan target) {
        FFmpeg.Codec.av_packet_unref(packet);
        FFmpeg.Util.av_frame_unref(frame);
        long targetTimeStamp = FFmpeg.Util.av_rescale_q(target.Ticks, TimeSpanRational, timeBase);
        int result = FFmpeg.Format.avformat_seek_file(
            format,
            audioStreamIndex,
            long.MinValue,
            targetTimeStamp,
            long.MaxValue,
            0);
        CheckResult(result);
        FFmpeg.Codec.avcodec_flush_buffers(codec);
        FFmpeg.SWR.swr_close(resampler);
        result = FFmpeg.SWR.swr_init(resampler);
        CheckResult(result);
    }
    internal static unsafe void ReceiveFrames(
        AVCodecContext* codecContext,
        SwrContext* resampler,
        AVFrame* frame,
        Action<PacketData> consumer,
        int outputChannels,
        int bytesPerSample,
        AVRational timeBase
        ) {
        while (true) {
            int result = FFmpeg.Codec.avcodec_receive_frame(codecContext, frame);
            if (result == FFmpeg.Flags.AVERR_EAGAIN || result == FFmpeg.Flags.AVERR_EOF) {
                return;
            }
            CheckResult(result);
            try {
                _ = Convert(resampler, consumer, frame->NBSamples, outputChannels, bytesPerSample, frame->ExtendedData, frame->Pts, timeBase);
            }
            finally {
                FFmpeg.Util.av_frame_unref(frame);
            }
        }
    }
    internal static unsafe void FlushResampler(
        SwrContext* resampler,
        Action<PacketData> consumer,
        int outputChannels,
        int bytesPerSample,
        long pts,
        AVRational timeBase) {
        while (true) {
            int result = Convert(resampler, consumer, 0, outputChannels, bytesPerSample, null, pts, timeBase);
            if (result > 0) {
                return;
            }
        }
    }
    private static unsafe int Convert(
        SwrContext* resampler,
        Action<PacketData> consumer,
        int inputSamples,
        int outputChannels,
        int bytesPerSample,
        byte** inputPointer,
        long pts,
        AVRational timeBase) {
        int outputCapacity = FFmpeg.SWR.swr_get_out_samples(resampler, inputSamples);
        CheckResult(outputCapacity);
        if (outputCapacity == 0) {
            return 1;
        }
        int outputBytes = outputCapacity * outputChannels * bytesPerSample;
        byte* outputBuffer = (byte*)NativeMemory.Alloc((nuint)outputBytes);
        if (outputBuffer == null) {
            throw new OutOfMemoryException();
        }
        try {
            byte* outputPointer = outputBuffer;
            int convertedSamples = FFmpeg.SWR.swr_convert(
                resampler,
                &outputPointer,
                outputCapacity,
                inputPointer,
                inputSamples);
            CheckResult(convertedSamples);
            if (convertedSamples == 0) {
                return 0;
            }
            int convertedBytes = convertedSamples * outputChannels * bytesPerSample;
            ReadOnlySpan<byte> span = new(outputBuffer, convertedBytes);

            long ticks = FFmpeg.Util.av_rescale_q(pts, timeBase, TimeSpanRational);
            consumer(new(span, ticks));
            return 0;
        }
        finally {
            NativeMemory.Free(outputBuffer);
        }
    }
}
