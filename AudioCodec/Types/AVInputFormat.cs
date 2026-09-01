using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVInputFormat {
    public byte* Name;
    public byte* LongName;
    public int Flags;
    public byte* Extensions;
    public AVCodecTag** CodecTag;
    public AVClass* PrivateClass;
    public byte* MimeType;
}
