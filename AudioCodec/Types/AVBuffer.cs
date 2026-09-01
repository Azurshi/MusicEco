using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVBuffer {
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVBufferRef {
    public AVBuffer* Buffer;
    public byte* Data;
    public ulong Size;
}
