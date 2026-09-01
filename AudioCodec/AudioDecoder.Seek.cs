using AudioCodec.Types;

namespace AudioCodec;

public partial class AudioDecoder {
    internal unsafe void SeekDecoder(
        AVFormatContext* format,
        AVCodecContext* codec,
        SwrContext* resampler,
        AVPacket* packet,
        AVFrame* frame,
        int audioStreamIndex,
        AVRational timeBase,
        TimeSpan target
        ) {
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
        FFmpegUtility.CheckResult(result);
        FFmpeg.Codec.avcodec_flush_buffers(codec);
        FFmpeg.SWR.swr_close(resampler);
        result = FFmpeg.SWR.swr_init(resampler);
        FFmpegUtility.CheckResult(result);
    }
}
