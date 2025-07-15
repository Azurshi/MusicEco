using System.Diagnostics;
namespace DataStorage.DataAccess; 
internal class ImageManager {
    private static long cachedSize = 0;
#if WINDOWS
    internal static readonly SemaphoreSlim semaphore = new(2);
#elif ANDROID
    internal static readonly SemaphoreSlim semaphore = new(1);
#endif
    private static string GetFileExtension(string filePath) {
        return System.IO.Path.GetExtension(filePath);
    }
    private static async Task<TagLib.File> ReadFile(string filePath) {
        await semaphore.WaitAsync();
        try {
#if WINDOWS
            TagLib.File file = await Task.Run(() => TagLib.File.Create(filePath, TagLib.ReadStyle.PictureLazy));
#elif ANDROID
            TagLib.File file = await Task.Run(() => ScannerFileHelper.TagLibCreatFile(Android.Net.Uri.Parse(filePath)!));
#endif
            return file;
        }
        finally {
            semaphore.Release();
        }
    }
    private static async Task<TagLib.ByteVector?> ReadData(string filePath) {
        try {
            string fileExtension = GetFileExtension(filePath);
            if (Domain.Config.SupportedExtensions.Contains(fileExtension)) {
                TagLib.File file = await ReadFile(filePath);
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
    private static async Task<TagLib.ByteVector?> GetData(string filePath) {
        if (_cachedData.TryGetValue(filePath, out TagLib.ByteVector? cachedData)) {
            // Found cached, pass
            return cachedData;
        }
        else {
            TagLib.ByteVector? data = await ReadData(filePath);
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
    internal static async Task<ImageSource> ExtractImage(string filePath) {
        TagLib.ByteVector? data = await GetData(filePath);
        if (data != null) {
            if (_cachedImages.TryGetValue(data, out WeakReference<ImageSource>? cachedImageRef)) {
                if (cachedImageRef.TryGetTarget(out ImageSource? cachedImage)) {
                    // Found cached and not GC
                    return cachedImage;
                }
                else {
                    // Found cached but released by GC
                    Debug.WriteLine($"Cache hit GC {filePath}");
                    ImageSource image = ImageSource.FromStream(() => new MemoryStream(data.Data));
                    _cachedImages[data] = new WeakReference<ImageSource>(image);
                    return image;
                }
            }
            else {
                // Not cached
                Debug.WriteLine($"Cache miss {filePath}");
                ImageSource image = ImageSource.FromStream(() => new MemoryStream(data.Data));
                _cachedImages[data] = new WeakReference<ImageSource>(image);
                return image;
            }
        }
        else {
            return DefaultImageSource;
        }
    }
    private static readonly Dictionary<TagLib.ByteVector, WeakReference<ImageSource>> _cachedIcons = [];
    private static async Task<ImageSource> ConvertIcon(TagLib.ByteVector data) {
        return await ImageProcessing.ScaleImageToIcon(data, new(256, 256));
    }
    internal static async Task<ImageSource> ExtractIcon(string filePath) {
        TagLib.ByteVector? data = await GetData(filePath);
        if (data != null) {
            if (_cachedIcons.TryGetValue(data, out WeakReference<ImageSource>? cachedImageRef)) {
                if (cachedImageRef.TryGetTarget(out ImageSource? cachedImage)) {
                    // Found cached and not GC
                    return cachedImage;
                }
                else {
                    // Found cached but released by GC
                    Debug.WriteLine($"Cache hit GC {filePath}");
                    ImageSource image = await ConvertIcon(data.Data);
                    _cachedIcons[data] = new WeakReference<ImageSource>(image);
                    return image;
                }
            }
            else {
                // Not cached
                Debug.WriteLine($"Cache miss {filePath}");
                ImageSource image = await ConvertIcon(data.Data);
                _cachedIcons[data] = new WeakReference<ImageSource>(image);
                return image;
            }
        }
        else {
            return DefaultIconSource;
        }
    }
    private static readonly ImageSource DefaultImageSource = ImageSource.FromFile("default_image.png");
    private static readonly ImageSource DefaultIconSource = ImageSource.FromFile("default_icon.png");
}
