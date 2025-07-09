using System.Diagnostics;
namespace DataStorage.DataAccess; 
internal class ImageManager {
    private static long cachedSize = 0;
    private static string GetFileExtension(string filePath) {
        return System.IO.Path.GetExtension(filePath);
    }
    private static TagLib.ByteVector? ReadData(string filePath) {
        try {
            string fileExtension = GetFileExtension(filePath);
            if (Config.AudioFileExtensions.Contains(fileExtension)) {
#if WINDOWS
                TagLib.File file = TagLib.File.Create(filePath, TagLib.ReadStyle.PictureLazy);
#elif ANDROID
                TagLib.File file = ScannerFileHelper.TagLibCreatFile(Android.Net.Uri.Parse(filePath)!);
#endif
                if (file.Tag.Pictures.Length > 0) {
                    return file.Tag.Pictures[0].Data;
                }
                else {
                    return null;
                }
            }
            else {
                Debug.WriteLine($"Failed to read image: {filePath}");
                return null;
            }
        }
        catch {
            return null;
        }
    }
    private static readonly HashSet<TagLib.ByteVector> _reservedCachedData = [];
    private static readonly Dictionary<string, TagLib.ByteVector> _cachedData = [];
    private static TagLib.ByteVector? GetData(string filePath) {
        if (_cachedData.TryGetValue(filePath, out TagLib.ByteVector? cachedData)) {
            // Found cached, pass
            return cachedData;
        }
        else {
            TagLib.ByteVector? data = ReadData(filePath);
            if (data != null) {
                if(_reservedCachedData.TryGetValue(data, out TagLib.ByteVector? loadedData)) {
                    // Not direct cached, but loaded
                    // Cause duplicate memory if cache data directly
                    _cachedData[filePath] = loadedData;
                    Debug.WriteLine($"New reversed cached {_reservedCachedData.Count} / {_cachedData.Count} | {filePath}");
                    return data;
                } else {
                    // Not cached or loaded
                    cachedSize += data.Data.LongLength;
                    Debug.WriteLine($"New cached {_reservedCachedData.Count} / {_cachedData.Count} = {(double)cachedSize/1024:F4} KB | {filePath}");
                    _cachedData[filePath] = data;
                    _reservedCachedData.Add(data);
                    return data;
                }
            } else {
                // Null, return
                return null;
            }
        }
    }

    private static readonly Dictionary<TagLib.ByteVector, WeakReference<ImageSource>> _cachedImages = [];
    internal static ImageSource ExtractImage(string filePath) {
        TagLib.ByteVector? data = GetData(filePath);
        if (data != null) {
            if (_cachedImages.TryGetValue(data, out WeakReference<ImageSource>? cachedImageRef)) {
                if (cachedImageRef.TryGetTarget(out ImageSource? cachedImage)) {
                    // Found cached and not GC
                    return cachedImage;
                } else {
                    // Found cached but released by GC
                    Debug.WriteLine($"Cache hit GC {filePath}");
                    ImageSource image = ImageSource.FromStream(() => new MemoryStream(data.Data));
                    _cachedImages[data] = new WeakReference<ImageSource>(image);
                    return image;
                }
            } else {
                // Not cached
                Debug.WriteLine($"Cache miss {filePath}");
                ImageSource image = ImageSource.FromStream(() => new MemoryStream(data.Data));
                _cachedImages[data] = new WeakReference<ImageSource>(image);
                return image;
            }
        } else {
            return DefaultImageSource;
        }
    }
    private static readonly ImageSource DefaultImageSource = ImageSource.FromFile("default_image.png");
}
