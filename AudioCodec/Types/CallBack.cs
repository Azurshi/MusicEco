using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe partial struct AVIOInterruptCB {
    public IntPtr Callback;
    public void* Opaque;
}