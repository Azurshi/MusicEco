using AudioCodec.Enum;
using AudioCodec.Types;
using System.Runtime.InteropServices;

namespace AudioCodec.FFmpeg;

#pragma warning disable CA1401, SYSLIB1054
public static unsafe class SWR {
    public const string LibName = Config.AVResample;
    public const CallingConvention CC = CallingConvention.Cdecl;

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int swr_alloc_set_opts2(
        SwrContext** context,
        AVChannelLayout* outputLayout,
        AVSampleFormat outputFormat,
        int outputSampleRate,
        AVChannelLayout* inputLayout,
        AVSampleFormat inputFormat,
        int inputSampleRate,
        int logOffset,
        void* logContext);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern int swr_init(
        SwrContext* context);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int swr_convert(
        SwrContext* context,
        byte** output,
        int outputSampleCount,
        byte** input,
        int inputSampleCount);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern void swr_free(
        SwrContext** context);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void swr_close(
        SwrContext* context);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int swr_get_out_samples(
        SwrContext* context,
        int inputSamples);


}
#pragma warning restore CA1401, SYSLIB1054