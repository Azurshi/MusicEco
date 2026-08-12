using MusicEco.Core.Services;
using MusicEco.Image.Decoder;
using SkiaSharp;

namespace MusicEco.Services;

public partial class IconService {
    private readonly Dictionary<CoverSize, IDecodeResult> _default = [];
    private static void ThrowInitializeErrorIf(bool condition) {
        if (condition) {
            throw new Exception($"Failed to initialize {nameof(IconService)}");
        }
    }
    public async Task InitializeDefault(IServiceProvider provider) {
        using (var file = await FileSystem.OpenAppPackageFileAsync("default_image_raw.png")) {
            using(var memory = new MemoryStream()) {
                await file.CopyToAsync(memory);
                var imageDecoder = provider.GetRequiredService<IImageDecoder>();
                byte[] data = memory.ToArray();
                var smallResult =  imageDecoder.Decode(data, Data.Config.SmallIconSize, false);
                ThrowInitializeErrorIf(!smallResult.Success);
                var mediumResult = imageDecoder.Decode(data, Data.Config.MediumIconSize, false);
                ThrowInitializeErrorIf(!mediumResult.Success);
                var largeResult = imageDecoder.Decode(data, Data.Config.LargeIconSize, false);
                ThrowInitializeErrorIf(!largeResult.Success);
                this._default[CoverSize.Small] = smallResult;
                this._default[CoverSize.Medium] = mediumResult;
                this._default[CoverSize.Large] = largeResult;
            }
        }
    }
}
