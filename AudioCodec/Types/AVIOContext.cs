using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVIOContext {
    public AVClass* Class;
    public byte* Buffer;
    public int BufferSize;
    public byte* BufferPtr;
    public byte* BufferEnd;
    public void* Opaque;
    public IntPtr ReadPacket;
    public IntPtr WritePacket;
    public IntPtr SeekPacket;
    public long Pos;
    public int EOFReached;
    public int Error;
    public int WriteFlag;
    public int MaxPacketSize;
    public int MinPacketSize;
    public ulong Checksum;
    public byte* ChecksumPtr;
    public IntPtr UpdateChecksum;
    public IntPtr ReadPause;
    public IntPtr ReadSeek;
    public int Seekable;
    public int Direct;
    public byte* ProtocolWhilelist;
    public byte* ProtocolBlacklist;
    public IntPtr WriteDataType;
    public int IgnoreBoundaryPoint;
    public byte* BufferPtrMax;
    public long ByteRead;
    public long ByteWritten;
}
