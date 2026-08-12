using AudioCodec.Enum;
using AudioCodec.Managed;
using AudioCodec.Types;
using AudioCodec.Utility;

namespace AudioCodec;

public partial class AudioDecoder {
    internal unsafe void DecodeToPCM(
        Stream input,
        CancellationToken token
        ) {
        using(var managedFormat = new FormatFromStream(input, Config.IOBufferSize)) {
            AVFormatContext* format = managedFormat.Context;
            AVCodecContext* codecContext = null;
            AVPacket* packet = null;
            AVFrame* frame = null;
            SwrContext* resampler = null;
            try {
                int result = FFmpeg.Format.avformat_find_stream_info(format, null);
                FFmpegUtility.CheckResult(result);
                Volatile.Write(ref this.DurationTicks, format->Duration * 10);
                int audioStreamIndex = FFmpegUtility.FindStreamIndex(format, AVMediaType.AUDIO);
                if (audioStreamIndex < 0) {
                    throw new InvalidOperationException("No audio stream found");
                }
                AVStream* stream = format->Streams[audioStreamIndex];
                AVCodecParameters* parameters = stream->CodecParameters;
                AVCodec* decoder = FFmpeg.Codec.avcodec_find_decoder(parameters->CodecId);
                if (decoder == null) {
                    throw new InvalidOperationException("");
                }
                codecContext = FFmpeg.Codec.avcodec_alloc_context3(decoder);
                result = FFmpeg.Codec.avcodec_parameters_to_context(codecContext, parameters);
                FFmpegUtility.CheckResult(result);
                result = FFmpeg.Codec.avcodec_open2(codecContext, decoder, null);
                FFmpegUtility.CheckResult(result);
                AVChannelLayout outputLayout = default;
                FFmpeg.Util.av_channel_layout_default(&outputLayout, Config.OutputChannels);
                result = FFmpeg.SWR.swr_alloc_set_opts2(
                    &resampler,
                    &outputLayout,
                    Config.OutputFormat,
                    Config.OutputSampleRate,
                    &codecContext->ChannelLayout,
                    codecContext->SampleFormat,
                    codecContext->SampleRate,
                    0,
                    null);
                FFmpegUtility.CheckResult(result);
                result = FFmpeg.SWR.swr_init(resampler);
                FFmpegUtility.CheckResult(result);
                packet = FFmpeg.Codec.av_packet_alloc();
                frame = FFmpeg.Util.av_frame_alloc();
                AVRational timeBase = stream->TimeBase;
                while(true) {
                    result = this.DecodeLoop(
                        format, codecContext, resampler, 
                        packet, frame, 
                        audioStreamIndex, timeBase,
                        token);
                    if (result < 0) {
                        break;
                    }
                }
                // Flush
                _ = FFmpeg.Codec.avcodec_send_packet(codecContext, null);
                this.ReceiveFrames(codecContext, resampler, frame, timeBase, true);
                this.FlushResampler(resampler, frame->Pts, timeBase);
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
}
