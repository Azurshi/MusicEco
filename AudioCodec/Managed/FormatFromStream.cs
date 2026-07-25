using AudioCodec.Types;
using System.Runtime.InteropServices;

namespace AudioCodec.Managed;

internal unsafe sealed class FormatFromStream: IDisposable {
    private static readonly FFmpeg.Format.ReadPacketCallback ReadPacketCallBack = ReadPacket;
    private static readonly FFmpeg.Format.SeekPacketCallback SeekPacketCallback = SeekPacket;
    private AVFormatContext* _formatContext;
    private AVIOContext* _ioContext;
    private GCHandle _streamHandle;
    private bool _disposed = false;
    public AVFormatContext* Context => this._formatContext;
    public FormatFromStream(Stream inputStream, int bufferSize) {
        byte* buffer = (byte*)FFmpeg.Util.av_malloc((uint)bufferSize);
        if (buffer == null) {
            throw new OutOfMemoryException();
        }
        AVFormatContext* format = FFmpeg.Format.avformat_alloc_context();
        if (format == null) {
            FFmpeg.Util.av_free(buffer);
            throw new OutOfMemoryException();
        }
        GCHandle handle = GCHandle.Alloc(inputStream);
        AVIOContext* io = FFmpeg.Format.avio_alloc_context(
            buffer,
            bufferSize,
            0, // ReadOnly
            (void*)GCHandle.ToIntPtr(handle),
            ReadPacketCallBack,
            null,
            SeekPacketCallback);
        if (io == null) {
            FFmpeg.Util.av_free(buffer);
            FFmpeg.Format.avformat_free_context(format);
            handle.Free();
            throw new OutOfMemoryException();
        }
        format->PB = io;
        format->Flags |= FFmpeg.Flags.AVFMT_FLAG_CUSTOM_IO;
        int result = FFmpeg.Format.avformat_open_input(
            &format,
            null,
            null,
            null);
        if (result < 0) {
            FFmpeg.Format.avformat_close_input(&format);
            FFmpeg.Format.avio_context_free(&io);
            handle.Free();
            throw new Exception($"FFmpeg error: {result}");
        } else {
            this._formatContext = format;
            this._ioContext = io;
            this._streamHandle = handle;
        }
    }
    public void Dispose() {
        if (this._disposed) {
            return;
        }
        this._disposed = true;
        if (this._formatContext != null) {
            AVFormatContext* format = _formatContext;
            FFmpeg.Format.avformat_close_input(&format);
            this._formatContext = null;
        }
        if (this._ioContext != null) {
            AVIOContext* io = _ioContext;
            FFmpeg.Format.avio_context_free(&io);
            this._ioContext = null;
        }
        if (this._streamHandle.IsAllocated) {
            this._streamHandle.Free();
        }
    }
    private static int ReadPacket(
        void* opaque,
        byte* buffer,
        int bufferSize) {
        try {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)opaque);
            var stream = (Stream)handle.Target!;
            Span<byte> span = new(buffer, bufferSize);
            int count = stream.Read(span);
            if (count == 0) {
                return FFmpeg.Flags.AVERR_EOF;
            }
            else {
                return count;
            }
        }
        catch {
            return FFmpeg.Flags.AVERR_EIO;
        }
    }
    private static long SeekPacket(
        void* opaque,
        long offset,
        int whence) {
        GCHandle handle = GCHandle.FromIntPtr((IntPtr)opaque);
        var stream = (Stream)handle.Target!;
        if ((whence & FFmpeg.Flags.AVSEEK_SIZE) != 0) {
            return stream.CanSeek ? stream.Length : -1;
        }
        int origin = whence & 0xFFFF; // Remove extra flags
        SeekOrigin seekOrigin = origin switch {
            FFmpeg.Flags.SEEK_SET => SeekOrigin.Begin,
            FFmpeg.Flags.SEEK_CUR => SeekOrigin.Current,
            FFmpeg.Flags.SEEK_END => SeekOrigin.End,
            _ => throw new ArgumentOutOfRangeException(nameof(whence))
        };
        return stream.Seek(offset, seekOrigin);
    }
}
