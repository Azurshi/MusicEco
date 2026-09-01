using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVHWAccel {
    public byte* Name;
    public AVMediaType Type;
    public AVCodecID Id;
    public AVPixelFormat PixelFormat;
    public int Capabilities;
}
