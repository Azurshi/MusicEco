using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVPacket {
    public AVBufferRef* Buffer;
    public long Pts;
    public long Dts;
    public byte* Data;
    public int Size;
    public int StreamIndex;
    public int Flags;
    public AVPacketSideData* SideData;
    public int SideDataElements;
    public long Duration;
    public long Pos;
    public void* Opaque;
    public AVBufferRef* OpaqueRef;
    public AVRational TimeBase;
}
