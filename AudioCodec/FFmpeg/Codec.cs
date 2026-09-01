using AudioCodec.Enum;
using AudioCodec.Types;
using System.Runtime.InteropServices;

namespace AudioCodec.FFmpeg;

#pragma warning disable CA1401, SYSLIB1054
public static unsafe class Codec {
    public const string LibName = Config.AVCodec;
    public const CallingConvention CC = CallingConvention.Cdecl;

    [DllImport(LibName, CallingConvention = CC)]
    public static extern AVCodecContext* avcodec_alloc_context3(
        AVCodec* codec);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void avcodec_free_context(
        AVCodecContext** context);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern AVPacket* av_packet_alloc();
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void av_packet_free(
        AVPacket** packet);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void av_packet_unref(
        AVPacket* packet);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int avcodec_open2(
        AVCodecContext* codecContext,
        AVCodec* codec,
        AVDictionary** options);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int avcodec_send_packet(
        AVCodecContext* context,
        AVPacket* packet);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern int avcodec_receive_frame(
        AVCodecContext* context,
        AVFrame* frame);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern AVCodec* avcodec_find_decoder(
        AVCodecID codecId);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern byte* avcodec_find_name(
        AVCodecID codecId);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern int avcodec_parameters_to_context(
        AVCodecContext* codecContext,
        AVCodecParameters* parameters);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern void avcodec_flush_buffers(
        AVCodecContext* context);
}
#pragma warning restore CA1401, SYSLIB1054