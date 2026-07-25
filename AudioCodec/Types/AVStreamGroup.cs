using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVStreamGroup {
    public AVClass* Class;
    public void* PrivateData;
    public uint Index;
    public long Id;
    public AVStreamGroupParamsType Type;
    public AVStreamGroup_params Params;
    public AVDictionary* Metadata;
    public uint NBStreams;
    public AVStream** Streams;
    public int Disposition;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVIAMFAudioElement { }
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVIAMFMixPresensation { }
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVStreamGroupTileGrid_offsets {
    public uint Idx;
    public int Horizontal;
    public int Vertical;
}
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVStreamGroupTileGrid {
    public AVClass* Class;
    public uint NBTiles;
    public int CodedWith;
    public int CodedHeight;
    public AVStreamGroupTileGrid_offsets* Offsets;
    public ByteArray4 Background;
    public int HorizontalOffset;
    public int VerticalOffset;
    public int Width;
    public int Height;
    public AVPacketSideData* CodedSideData;
    public int NBCodedSideData;
}
[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVStreamGroupLCEVC {
    public AVClass* Class;
    public uint LCEVC_Index;
    public int Width;
    public int Height;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct AVStreamGroup_params {
    [FieldOffset(0)]
    public AVIAMFAudioElement* IAMFAdioElement;
    [FieldOffset(0)]
    public AVIAMFMixPresensation* IAMFPresensation;
    [FieldOffset(0)]
    public AVStreamGroupTileGrid* TileGrid;
    [FieldOffset(0)]
    public AVStreamGroupLCEVC* LCEVC;
}