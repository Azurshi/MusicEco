using MusicEco.Core.Services;
using SkiaSharp;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MusicEco.Image.Decoder;

internal class SkiaImageDecoder: BaseImageCodec, IImageDecoder {
    public static SKImage DecodeInner(Memory<byte> data, Vector2 targetSize, bool highQuality) {
        if (!MemoryMarshal.TryGetArray(data, out ArraySegment<byte> segment)) {
            throw new InvalidOperationException();
        }
        using (var stream = new SKMemoryStream(segment.Array!)) {
            using (var codec = SKCodec.Create(stream)) {
                float scale = Math.Min(targetSize.X / codec.Info.Width, targetSize.Y / codec.Info.Height);
                var scaledDimension = codec.GetScaledDimensions(scale);
                var info = codec.Info.WithSize(scaledDimension.Width, scaledDimension.Height).WithColorType(SKColorType.Bgra8888);
                using (var bitmap = new SKBitmap(info)) {
                    codec.GetPixels(info, bitmap.GetPixels());
                    return SKImage.FromBitmap(bitmap);
                }
            }
        }
    }

    public IDecodeResult Decode(Memory<byte> data, Vector2 maxSize, bool highQuality) {
        try {
            var image = DecodeInner(data, maxSize, highQuality);
            return new SkiaDecodeResult(image);
        }
        catch {
            return new FailedDecodeResult();
        }
    }

    public async Task<IDecodeResult> DecodeAsync(Memory<byte> data, Vector2 maxSize, bool highQuality) {
        if (Semaphore == null) {
            throw new Exception("Object not initalized");
        }
        await Semaphore.WaitAsync();
        try {
            var image = await Task.Run(() => DecodeInner(data, maxSize, highQuality));
            return new SkiaDecodeResult(image);
        }
        catch {
            return new FailedDecodeResult();
        }
        finally {
            Semaphore.Release();
        }
    }
}
