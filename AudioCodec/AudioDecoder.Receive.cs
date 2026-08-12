using AudioCodec.Types;

namespace AudioCodec;

public partial class AudioDecoder {
    internal unsafe void ReceiveFrames(
        AVCodecContext* codecContext, 
        SwrContext* resampler,
        AVFrame* frame,
        AVRational timeBase,
        bool flush
        ) {
        while(true) {
            int result = FFmpeg.Codec.avcodec_receive_frame(codecContext, frame);
            if (result == FFmpeg.Flags.AVERR_EAGAIN 
                || result == FFmpeg.Flags.AVERR_EOF) {
                return;
            }
            FFmpegUtility.CheckResult(result);
            try {
                _ = this.Convert(resampler, frame->ExtendedData, frame->NBSamples, frame->Pts, timeBase, flush);
            }
            finally {
                FFmpeg.Util.av_frame_unref(frame);
            }
        }
    }
    internal unsafe void FlushResampler(
        SwrContext* resampler,
        long pts,
        AVRational timeBase
        ) {
        while(true) {
            int result = this.Convert(resampler, null, 0, pts, timeBase, true);
            if (result < 0) {
                return;
            }
        }
    }
}
