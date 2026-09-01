using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVChapter {
    public long Id;
    public AVRational TimeBase;
    public long Start;
    public long End;
    public AVDictionary* Metadata;
}

