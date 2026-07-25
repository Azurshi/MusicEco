using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVPacketSideData {
    public byte* Data;
    public ulong Size;
    public AVPacketSideDataType Type;
}
