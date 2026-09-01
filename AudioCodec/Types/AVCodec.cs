using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVCodec {
    public byte* Name;
    public byte* LongName;
    public AVMediaType Type;
    public AVCodecID Id;
    public int Capabilities;
    public byte MaxLowres;
    [Obsolete]
    public AVRational* SupportedFrameRates;
    [Obsolete]
    public AVPixelFormat PixelFormat;
    [Obsolete]
    public int* SupportedSampleRates;
    [Obsolete]
    public AVSampleFormat SampleFormat;
    public AVClass* PrivateClass;
    public AVProfile* Profiles;
    public byte* WrapperName;
    [Obsolete]
    public AVChannelLayout* ChannelLayout;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVCodecParameters {
    public AVMediaType Type;
    public AVCodecID CodecId;
    public uint CodecTag;
    public byte* ExtraData;
    public int ExtraDataSize;
    public AVPacketSideData* CodedSideData;
    public int NBCodedSizeData;
    public int Format;
    public long BitRate;
    public int BitPerCodedSample;
    public int BitPerRawSample;
    public int Profile;
    public int Level;
    public int Width;
    public int Height;
    public AVRational SampelAspectRatio;
    public AVRational FrameRate;
    public AVFieldOrder FieldOrder;
    public AVColorRange ColorRange;
    public AVColorPrimaries ColorPrimaries;
    public AVColorTransferCharacteristic ColorTransferCharacteristic;
    public AVColorSpace ColorSpace;
    public AVChromaLocation ChromaLocation;
    public int VideoDelay;
    public AVChannelLayout ChannelLayout;
    public int SampleRate;
    public int BlockAlign;
    public int FrameSize;
    public int IntialPadding;
    public int TrailingPadding;
    public int SeekReroll;
    public AVAlphaMode AlphaMode;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVCodecDescriptor {
    public AVCodecID Id;
    public AVMediaType Type;
    public byte* Name;
    public byte* LongName;
    public int Properties;
    public byte** MimeTypes;
    public AVProfile* Profiles;
}