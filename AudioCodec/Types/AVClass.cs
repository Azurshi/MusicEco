using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVClass {
    public byte* ClassName;
    public byte* ItemName;
    public AVOption* Option;
    public int Version;
    public int LogLevelOffsetOffset;
    public int LogLevelContextOffset;
    public AVClassCategory Category;
    public IntPtr GetCategory;
    public IntPtr QueryRanges;
    public IntPtr ChildNext;
    public IntPtr ChildClassIterate;
    public int StateFlagsOffset;
}
