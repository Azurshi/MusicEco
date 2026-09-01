using AudioCodec.Enum;
using System.Runtime.InteropServices;

namespace AudioCodec.Types;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVProgram {
    public int Id;
    public int Flags;
    public AVDiscard Discard;
    public uint* StreamIndex;
    public uint NBStreamIndexes;
    public AVDictionary* Metadata;
    public int ProgramNum;
    public int PMT_Pid;
    public int PCR_Pid;
    public int PMT_Version;
    public long StartTime;
    public long EndTime;
    public long PTSWrapReference;
    public int PTSWrapBehavior;
}
