using MusicEco.Core.Services;

namespace MusicEco.Data;

internal class IconEncoderBuffer {
    public byte[] SmallIconBuffer = new byte[Config.SmallIconBufferSize];
    public byte[] MediumIconBuffer = new byte[Config.MediumIconBufferSize];
    public byte[] LargeIconBuffer = new byte[Config.LargeIconBufferSize];
    public Memory<byte> GetSmallIcon(int length) {
        return SmallIconBuffer.AsMemory(0, length);
    }
    public Memory<byte> GetSmallIcon(IconResult result) {
        return SmallIconBuffer.AsMemory(0, result.SmallLength);
    }
    public Memory<byte> GetMediumIcon(int length) {
        return MediumIconBuffer.AsMemory(0, length);
    }
    public Memory<byte> GetMediumIcon(IconResult result) {
        return MediumIconBuffer.AsMemory(0, result.MediumLength);
    }
    public Memory<byte> GetLargeIcon(int length) {
        return LargeIconBuffer.AsMemory(0, length);
    }
    public Memory<byte> GetLargeIcon(IconResult result) {
        return LargeIconBuffer.AsMemory(0, result.LargeLength);
    }
}