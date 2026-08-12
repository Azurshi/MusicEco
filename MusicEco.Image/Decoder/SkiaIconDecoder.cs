using MusicEco.Core.Services;
using SkiaSharp;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using SkiaSharp.Views.Maui.Controls;

namespace MusicEco.Image.Decoder;

internal class SkiaIconDecoder: BaseImageCodec, IIconDecoder {
    public static SKImage DecodeInner(Memory<byte> data) {
        if (!MemoryMarshal.TryGetArray(data, out ArraySegment<byte> segment)) {
            throw new InvalidOperationException();
        }
        using (var stream = new SKMemoryStream(segment.Array!)) {
            using (var codec = SKCodec.Create(stream)) {
                var info = codec.Info.WithColorType(SKColorType.Bgra8888);
                using (var bitmap = new SKBitmap(info)) {
                    codec.GetPixels(info, bitmap.GetPixels());
                    return SKImage.FromBitmap(bitmap);
                }
            }   
        }
    }
    public IDecodeResult Decode(Memory<byte> data) {
        try {
            var image = DecodeInner(data);
            return new SkiaDecodeResult(image);
        } 
        catch {
            return new FailedDecodeResult();
        }
    }
    public async Task<IDecodeResult> DecodeAsync(Memory<byte> data) {
        if (Semaphore == null) {
            throw new Exception("Object not initalized");
        }
        await Semaphore.WaitAsync();
        try {
            var image = await Task.Run(() => DecodeInner(data));
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
