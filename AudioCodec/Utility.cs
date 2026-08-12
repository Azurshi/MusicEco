using AudioCodec.Enum;
using AudioCodec.Types;

namespace AudioCodec;

internal static class FFmpegUtility {
    internal static void CheckResult(int result) {
        if (result < 0) {
            throw new Exception($"FFmpeg error: {result}");
        }
    }
    internal static unsafe int FindStreamIndex(AVFormatContext* format, AVMediaType mediaType) {
        for (uint i = 0; i < format->NBStream; i++) {
            if (format->Streams[i]->CodecParameters->Type == mediaType) {
                return (int)i;
            }
        }
        return -1;
    }
    internal static int GetBytesPerSample(AVSampleFormat format) {
        return format switch {
            AVSampleFormat.U8 => 1,
            AVSampleFormat.S16 => 2,
            AVSampleFormat.S32 => 4,
            AVSampleFormat.S64 => 8,
            //AVSampleFormat.S16P => 2,
            //AVSampleFormat.S32P => 4,
            //AVSampleFormat.S64P => 8,
            AVSampleFormat.FLT => sizeof(float),
            AVSampleFormat.DBL => sizeof(double),
            //AVSampleFormat.FLTP => sizeof(float),
            //AVSampleFormat.DBLP => sizeof(double),
            _ => throw new ArgumentException($"AVSampleFormat.{format} not supported")
        };

    }
}
