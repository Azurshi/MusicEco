using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVOutputFormat {
    public byte* Name;
    public byte* LongName;
    public byte* MimeType;
    public byte* Extensions;
    public AVCodecID AudioCodec;
    public AVCodecID VideoCodec;
    public AVCodecID SubtitleCodec;
    public int Flags;
    public AVCodecTag** CodecTag;
    public AVClass* PrivateClass;
}
