using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVStream {
    public AVClass* Class;
    public int Index;
    public int Id;
    public AVCodecParameters* CodecParameters;
    public void* PrivateData;
    public AVRational TimeBase;
    public long StartTime;
    public long Duration;
    public long NBFrames;
    public int Disposition;
    public AVDiscard Discard;
    public AVRational SampleAspectRatio;
    public AVDictionary* Metadata;
    public AVRational AVGFrameRate;
    public AVPacket AttachedPic;
    public int EventFlags;
    public AVRational RFrameRate;
    public int PTSWrapBits;
}
