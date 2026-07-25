using AudioCodec.Types;
using System.Runtime.InteropServices;

namespace AudioCodec.FFmpeg;

#pragma warning disable CA1401, SYSLIB1054
public static unsafe class Util {
    public const string LibName = Config.AVUtil;
    public const CallingConvention CC = CallingConvention.Cdecl;

    [DllImport(LibName, CallingConvention = CC)]
    public static extern void* av_malloc(nuint size);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void av_free(void* ptr);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern AVFrame* av_frame_alloc();
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void av_frame_free(AVFrame** frame);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void av_frame_unref(AVFrame* frame);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern AVDictionaryEntry* av_dict_get(
        AVDictionary* dictionary,
        byte* key,
        AVDictionaryEntry* previous,
        int flags);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern AVDictionaryEntry* av_dict_iterate(
        AVDictionary* dictionary,
        AVDictionaryEntry* previous);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern int av_dict_set(
        AVDictionary** dictionary,
        byte* key,
        byte* value,
        int flags);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void av_dict_free(
        AVDictionary** dictionary);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int av_stderr(
        int error,
        byte* errorBuffer,
        nuint errorBufferSize);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern void av_channel_layout_default(
        AVChannelLayout* layout,
        int channelCount);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern long av_rescale_q(
        long a,
        AVRational bq,
        AVRational cq);
}

#pragma warning restore CA1401, SYSLIB1054