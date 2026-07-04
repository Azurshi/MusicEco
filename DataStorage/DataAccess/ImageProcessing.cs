using Domain;
using SkiaSharp;
using System.Diagnostics;
using System.Threading;

namespace DataStorage.DataAccess;
internal static class ImageProcessing {
    private static byte[] ScaleImageFromByte(TagLib.ByteVector data, Vector2I size) {
        int newWidth = size.X;
        int newHeight = size.Y;
        using (MemoryStream stream = new(data.Data)) {
            using (SKBitmap originalBitmap = SKBitmap.Decode(stream)) {
                if (originalBitmap.Width > originalBitmap.Height) {
                    newHeight = (int)MathF.Floor(1.0f * originalBitmap.Height / originalBitmap.Width * newWidth);
                }
                else {
                    newWidth = (int)MathF.Floor(1.0f * originalBitmap.Width / originalBitmap.Height * newHeight);
                }
                SKBitmap scaledBitmap = originalBitmap.Resize(new SKImageInfo(newWidth, newHeight), SKSamplingOptions.Default);
                using (SKImage image = SKImage.FromBitmap(scaledBitmap)) {
                    using (MemoryStream imageStream = new()) {
                        image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(imageStream);
                        imageStream.Seek(0, SeekOrigin.Begin);
                        byte[] byteData = imageStream.ToArray();
                        return byteData;
                    }
                }
            }
        }
    }
    internal static async Task<ImageSource> ScaleImageToIcon(TagLib.ByteVector data, Vector2I size) {
        await ImageManager.semaphore.WaitAsync();
        try {
            byte[] iconData = await Task.Run(() => ScaleImageFromByte(data, size));
            return ImageSource.FromStream(() => new MemoryStream(iconData));

        }
        finally {
            ImageManager.semaphore.Release();
        }
    }
}
