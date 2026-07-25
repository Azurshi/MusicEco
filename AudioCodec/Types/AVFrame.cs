using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVFrame {
    public BytePointerArray8 Data;
    public IntArray8 LineSize;
    public byte** ExtendedData;
    public int Width;
    public int Height;
    public int NBSamples;
    public int Format;
    public AVPictureType PictureType;
    public AVRational SampleAspectRatio;
    public long Pts;
    public long PktDts;
    public AVRational TimeBase;
    public int Quality;
    public void* Opaque;
    public int RepeatPict;
    public int SampleRate;
    public AVBufferRefPtrArray8 Buffer;
    public AVBufferRef** ExtendedBuffer;
    public int NBExtendedBuffer;
    public AVFrameSideData** SideData;
    public int NBSideData;
    public int Flags;
    public AVColorRange ColorRange;
    public AVColorPrimaries ColorPrimaries;
    public AVColorTransferCharacteristic ColorTransactionCharacteristic;
    public AVColorSpace ColorSpace;
    public AVChromaLocation ChromaLocation;
    public long BestEffortTimeStamp;
    public AVDictionary* Metadata;
    public int DecodeErrorFlags;
    public AVBufferRef* HWFramesCtx;
    public AVBufferRef* OpaqueRef;
    public ulong CropTop;
    public ulong CropBottom;
    public ulong CropLeft;
    public ulong CropRight;
    public void* PrivateRef;
    public AVChannelLayout ChannelLayout;
    public long Duration;
    public AVAlphaMode AlphaMode;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVFrameSideData {
    public AVFrameSideDataType Type;
    public byte* Data;
    public ulong Size;
    public AVDictionary* Metadata;
    public AVBufferRef* Buffer;
}