using AudioCodec.Types;
using System.Runtime.InteropServices;

namespace AudioCodec.FFmpeg;

#pragma warning disable CA1401, SYSLIB1054
public static unsafe class Format {
    public const string LibName = Config.AVFormat;
    public const CallingConvention CC = CallingConvention.Cdecl;

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int avformat_open_input(
        AVFormatContext** context,
        byte* url,
        AVInputFormat* format,
        AVDictionary** options);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void avformat_close_input(
        AVFormatContext** context);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern AVFormatContext* avformat_alloc_context();
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void avformat_free_context(
        AVFormatContext* context);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern AVIOContext* avio_alloc_context(
        byte* buffer,
        int bufferSize,
        int writeFlag,
        void* opaque,
        ReadPacketCallback? readPacket,
        WritePacketCallback? writePacket,
        SeekPacketCallback? seekPacket);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern void avio_context_free(
        AVIOContext** context);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int avformat_find_stream_info(
        AVFormatContext* context,
        AVOption** options);

    [DllImport(LibName, CallingConvention = CC)]
    public static extern int av_read_frame(
        AVFormatContext* format,
        AVPacket* packet);
    [DllImport(LibName, CallingConvention = CC)]
    public static extern int avformat_seek_file(
        AVFormatContext* context,
        int streamIndex,
        long minTimeStamp,
        long timeStamp,
        long maxTimeStamp,
        int flags);

    #region Callback
    [UnmanagedFunctionPointer(CC)]
    public delegate int ReadPacketCallback(
        void* opaque,
        byte* buffer,
        int bufferSize);
    [UnmanagedFunctionPointer(CC)]
    public delegate int WritePacketCallback(
        void* opaque,
        byte* buffer,
        int bufferSize);
    [UnmanagedFunctionPointer(CC)]
    public delegate long SeekPacketCallback(
        void* opaque,
        long offset,
        int whence);
    #endregion
}
#pragma warning restore CA1401, SYSLIB1054