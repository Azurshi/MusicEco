using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVChannelCustom {
    public AVChannel Id;
    public ByteArray16 Name;
    public void* Opaque;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVChannelLayout {
    public AVChannelOrder Order;
    public int NBChannels;
    public AVChannelLayout_u U;
    public void* Opaque;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct AVChannelLayout_u {
    [FieldOffset(0)]
    public ulong Mask;
    [FieldOffset(0)]
    public AVChannelCustom* Map;
}