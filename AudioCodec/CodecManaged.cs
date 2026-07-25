using System.Runtime.InteropServices;
using AudioCodec.Enum;
using AudioCodec.Types;


namespace AudioCodec;

public static partial class CodecManaged {
    public static void DecodeToPCMTest(Stream input, Stream output) {
        unsafe {
            Action<PacketData> callback = new((p) => {
                output.Write(p.Data);
            });
            DecodeToPCM(
                input, null, callback, (_) => { }, () => { },
                64 * 1024,
                44_100,
                2,
                AVSampleFormat.S16);
            
        }
    }
}