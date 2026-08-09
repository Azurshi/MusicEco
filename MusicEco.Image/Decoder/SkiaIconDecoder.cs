using MusicEco.Core.Services;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace MusicEco.Image.Decoder;

internal class SkiaIconDecoder: BaseImageCodec, IIconDecoder {
    public static byte[] DecodeInner(Memory<byte> data) {
        // Need testing
        if (!MemoryMarshal.TryGetArray(data, out ArraySegment<byte> segment)) {
            throw new InvalidOperationException();
        }
        using (var stream = new SKMemoryStream(segment.Array!)) {
            using (var codec = SKCodec.Create(stream)) {
                var info = codec.Info.WithColorType(SKColorType.Bgra8888);
                using (var bitmap = new SKBitmap(info)) {
                    codec.GetPixels(info, bitmap.GetPixels());
                    using (var image = SKImage.FromBitmap(bitmap)) {
                        using (var imageData = image.Encode(SKEncodedImageFormat.Webp, 90)) {
                            return imageData.ToArray();
                        }
                    }
                }
            }
        }
    }
    public byte[] Decode(Memory<byte> data) {
        return DecodeInner(data);
    }
    public async Task<byte[]> DecodeAsync(Memory<byte> data) {
        if (Semaphore == null) {
            throw new Exception("Object not initalized");
        }
        await Semaphore.WaitAsync();
        try {
            var bytes = await Task.Run(() => DecodeInner(data));
            return bytes;
        }
        finally {
            Semaphore.Release();
        }
    }
}
