using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVOption {
    public byte* Name;
    public byte* Help;
    public int Offset;
    public AVOptionType Type;
    public AVOptionDefaultValue DefaultValue;
    public double Min;
    public double Max;
    public int Flags;
    public byte* Unit;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVOptionArrayDef {
    public byte* Def;
    public uint SizeMin;
    public uint SizeMax;
    public byte Sep;
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct AVOptionDefaultValue {
    [FieldOffset(0)]
    public long Int64;
    [FieldOffset(0)]
    public double Double;
    [FieldOffset(0)]
    public byte* String;
    [FieldOffset(0)]
    public AVRational Q;
    [FieldOffset(0)]
    public AVOptionArrayDef* Array;
}