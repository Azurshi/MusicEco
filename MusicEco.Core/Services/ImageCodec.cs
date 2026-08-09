using System.Numerics;

namespace MusicEco.Core.Services;

public class IconResult(int smallLength, int mediumLength, int largeLength) {
    public int SmallLength { get; } = smallLength;
    public int MediumLength { get; } = mediumLength;
    public int LargeLength { get; } = largeLength; 
    public void ThrowIfEmpty() {
        if (SmallLength == 0 || MediumLength == 0 || LargeLength == 0) {
            throw new Exception("Result is empty");
        }
    }
}
public interface IImageCodec {
    public void Initialize(int nWorkers);
    public int NumWorkers { get; }
}
public interface IIconDecoder: IImageCodec {
    public byte[] Decode(Memory<byte> data);
    public Task<byte[]> DecodeAsync(Memory<byte> data); // ImageSource need to keep data alive so no buffer here
}
public interface IImageDecoder: IImageCodec {
    public byte[] Decode(Memory<byte> data, Vector2 maxSize, bool highQuality);
    public Task<byte[]> DecodeAsync(Memory<byte> data, Vector2 maxSize, bool highQuality);
}
public interface IIconEncoder: IImageCodec {
    public IconResult Encode(
        Memory<byte> data,
        Vector2 smallIconSize, Vector2 mediumIconSize, Vector2 largeIconSize,
        byte[] smallIconBuffer, byte[] mediumIconBuffer, byte[] largeIconBuffer);
    public Task<IconResult> EncodeAsync(
        Memory<byte> data,
        Vector2 smallIconSize, Vector2 mediumIconSize, Vector2 largeIconSize,
        byte[] smallIconBuffer, byte[] mediumIconBuffer, byte[] largeIconBuffer);
}
public interface IImageEncoder: IImageCodec {
    public int Encode(Memory<byte> data, string format, int quality, byte[] buffer);
    public Task<int> EncodeAsync(Memory<byte> data, string format, int quality, byte[] buffer);
}