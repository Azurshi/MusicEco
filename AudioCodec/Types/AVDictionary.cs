using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVDictionary {
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVDictionaryEntry {
    public byte* Key;
    public byte* Value;
}
