using AudioCodec.Types;
using System.Runtime.InteropServices;

namespace AudioCodec;

public partial class AudioDecoder {
    private static readonly AVRational TimeSpanRational = new() { Numerator = 1, Denominator = (int)TimeSpan.TicksPerSecond };
    /// <summary>
    /// Return negative when convert no sample, positive when success convert sample
    /// </summary>
    /// <returns></returns>
    /// <exception cref="OutOfMemoryException"></exception>
    internal unsafe int Convert(
        SwrContext* resampler, 
        byte** inputPointer, 
        int inputSamples, 
        long pts, 
        AVRational timeBase,
        bool flush
        ) {
        // This only upper capacity, not remaining data available
        int outputCapacity = FFmpeg.SWR.swr_get_out_samples(resampler, inputSamples);
        FFmpegUtility.CheckResult(outputCapacity);
        if (outputCapacity == 0) {
            return -1;
        }
        int outputBytes = Config.GetBytes(outputCapacity);
        // DYNAMIC ALLOCATION
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
            FFmpegUtility.CheckResult(convertedSamples);
            if (convertedSamples == 0) {
                return -1;
            }
            int convertedBytes = Config.GetBytes(convertedSamples);
            // FLush data
            if (!flush) {
                ReadOnlySpan<byte> span = new(outputPointer, convertedBytes);
                long ticks = FFmpeg.Util.av_rescale_q(pts, timeBase, TimeSpanRational);
                this.ConsumePacket(span, ticks);
            }
            return 1;
        }
        finally {
            NativeMemory.Free(outputBuffer);
        }

    }
}
