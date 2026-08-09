using MusicEco.Core.Services;
using System.Diagnostics;

namespace MusicEco.Services;

public partial class IconService {
    private readonly Dictionary<CoverSize, ImageSource> _default = [];
    public async Task InitializeDefault(IServiceProvider provider) {
        using (var file = await FileSystem.OpenAppPackageFileAsync("default_image_raw.png")) {
            using(var memory = new MemoryStream()) {
                await file.CopyToAsync(memory);
                var imageDecoder = provider.GetRequiredService<IImageDecoder>();
                byte[] data = memory.ToArray();
                var smallData =  imageDecoder.Decode(data, Data.Config.SmallIconSize, false);
                var mediumData = imageDecoder.Decode(data, Data.Config.MediumIconSize, false);
                var largeData = imageDecoder.Decode(data, Data.Config.LargeIconSize, false);
                this._default[CoverSize.Small] = ImageSource.FromStream(() => new MemoryStream(smallData));
                this._default[CoverSize.Medium] = ImageSource.FromStream(() => new MemoryStream(mediumData));
                this._default[CoverSize.Large] = ImageSource.FromStream(() => new MemoryStream(largeData));
            }
        }
    }
}
