using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct RCOverride {
    public int StartFrame;
    public int EndFrame;
    public int QScale;
    public float QualityFactor;
}
