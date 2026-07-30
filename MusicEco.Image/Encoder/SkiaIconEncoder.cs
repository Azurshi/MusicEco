using MusicEco.Core.Services;
using SkiaSharp;
using System.Numerics;

namespace MusicEco.Image.Encoder;

internal class SkiaIconEncoder:BaseImageCodec, IIconEncoder {
    private static int EncodeSingle(SKBitmap original, Vector2 size, byte[] buffer) {
        size = Common.ComputeSize(original.Width, original.Height, size);
        int width = (int)size.X;
        int height = (int)size.Y;
        using (var resized = original.Resize(new SKImageInfo(width, height), SKSamplingOptions.Default)) {
            using (var image = SKImage.FromBitmap(resized)) {
                using (var data = image.Encode(SKEncodedImageFormat.Webp, Config.IconQuality)) {
                    var span = data.AsSpan();
                    if (buffer.Length < span.Length) {
                        throw new Exception("Buffer overlow");
                    }
                    var bufferSpan = buffer.AsSpan()[..span.Length];
                    span.CopyTo(bufferSpan);
                    return span.Length;
                }
            }
        }
    }
    public IconResult Encode(
        Memory<byte> originalData, 
        Vector2 smallIconSize, Vector2 mediumIconSize, Vector2 largeIconSize,
        byte[] smallIconBuffer, byte[] mediumIconBuffer, byte[] largeIconBuffer) {
        using (var original = SKBitmap.Decode(originalData.Span)) {
            return new(
                EncodeSingle(original, smallIconSize, smallIconBuffer),
                EncodeSingle(original, mediumIconSize, mediumIconBuffer),
                EncodeSingle(original, largeIconSize, largeIconBuffer)
            );
        }
    }
    public async Task<IconResult> EncodeAsync(
        Memory<byte> data, 
        Vector2 smallIconSize, Vector2 mediumIconSize, Vector2 largeIconSize,
        byte[] smallIconBuffer, byte[] mediumIconBuffer, byte[] largeIconBuffer) {
        if (Semaphore == null) {
            throw new Exception("Object not initalized");
        }
        await Semaphore.WaitAsync();
        try {
            return await Task.Run(() => Encode(data, smallIconSize, mediumIconSize, largeIconSize, smallIconBuffer, mediumIconBuffer, largeIconBuffer));
        }
        finally {
            Semaphore.Release();
        }
    }
}
