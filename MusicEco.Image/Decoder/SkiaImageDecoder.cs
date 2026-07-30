using MusicEco.Core.Services;
using SkiaSharp;
using System.Numerics;
using System.Runtime.InteropServices;

namespace MusicEco.Image.Decoder;

internal class SkiaImageDecoder: BaseImageCodec, IImageDecoder {
    public static byte[] DecodeInner(Memory<byte> data, Vector2 targetSize, bool highQuality) {
        // Need testing
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
                    using (var image = SKImage.FromBitmap(bitmap)) {
                        if (highQuality) {
                            using (var imageData = image.Encode(SKEncodedImageFormat.Png, 100)) {
                                return imageData.ToArray();
                            }
                        }
                        else {
                            using (var imageData = image.Encode(SKEncodedImageFormat.Webp, Config.LowQuality)) {
                                return imageData.ToArray();
                            }
                        }
                    }
                }
            }
        }
    }
    public async Task<ImageSource> Decode(Memory<byte> data, Vector2 maxSize, bool highQuality) {
        if (Semaphore == null) {
            throw new Exception("Object not initalized");
        }
        await Semaphore.WaitAsync();
        try {
            var bytes = await Task.Run(() => DecodeInner(data, maxSize, highQuality));
            var stream = new MemoryStream(bytes);
            return ImageSource.FromStream(() => stream);
        }
        finally {
            Semaphore.Release();
        }
    }
}
