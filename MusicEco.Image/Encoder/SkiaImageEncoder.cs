using MusicEco.Core.Services;
using SkiaSharp;

namespace MusicEco.Image.Encoder;

internal class SkiaImageEncoder: BaseImageCodec, IImageEncoder {
    private static int EncodeInner(Memory<byte> original, SKEncodedImageFormat format, int quality, byte[] buffer) {
        using (var bitmap = SKBitmap.Decode(original.Span)) {
            using (var image = SKImage.FromBitmap(bitmap)) {
                using (var data = image.Encode(format, quality)) {
                    var span = data.AsSpan();
                    if (buffer.Length < span.Length) {
                        throw new Exception("Buffer overlow");
                    }
                    buffer = buffer[..span.Length];
                    span.CopyTo(buffer.AsSpan());
                    return span.Length;
                }
            }
        }
    }
    private static SKEncodedImageFormat GetFormat(string format) {
        return format switch {
            "png" => SKEncodedImageFormat.Png,
            "jpeg" => SKEncodedImageFormat.Jpeg,
            "jpg" => SKEncodedImageFormat.Jpeg,
            "jpegxl" => SKEncodedImageFormat.Jpegxl,
            "webp" => SKEncodedImageFormat.Webp,
            "avif" => SKEncodedImageFormat.Avif,
            "ico" => SKEncodedImageFormat.Ico,
            _ => SKEncodedImageFormat.Webp,
        };
    }
    public int Encode(Memory<byte> data, string format, int quality, byte[] buffer) {
        format = format.ToLower().Replace(".", "");
        var encodedFormat = GetFormat(format);
        return EncodeInner(data, encodedFormat, quality, buffer);
    }
    public async Task<int> EncodeAsync(Memory<byte> data, string format, int quality, byte[] buffer) {
        format = format.ToLower().Replace(".", "");
        var encodedFormat = GetFormat(format);
        if (Semaphore == null) {
            throw new Exception("Object not initalized");
        }
        await Semaphore.WaitAsync();
        try {
            return await Task.Run(() => EncodeInner(data, encodedFormat, quality, buffer));
        }
        finally {
            Semaphore.Release();
        }
    }
}
