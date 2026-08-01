using MusicEco.Core.Services;
using System.Diagnostics;

namespace MusicEco.Services;

public partial class IconService {
    private readonly Dictionary<CoverSize, ImageSource> _default = [];
    public async Task InitializeDefault(IServiceProvider provider) {
        using (var file = await FileSystem.OpenAppPackageFileAsync("default_image_raw.png")) {
            var imageDecoder = provider.GetRequiredService<IImageDecoder>();
            imageDecoder.Initialize(1);
            byte[] data = new byte[file.Length];
            file.ReadExactly(data);
            var smallIcon = await imageDecoder.Decode(data, Data.Config.SmallIconSize, false);
            var mediumIcon = await imageDecoder.Decode(data, Data.Config.MediumIconSize, false);
            var largeIcon = await imageDecoder.Decode(data, Data.Config.LargeIconSize, false);
            if (smallIcon == null || mediumIcon == null || largeIcon == null) {
                throw new System.Exception("IconService initialization failed");
            }
            this._default[CoverSize.Small] = smallIcon;
            this._default[CoverSize.Medium] = mediumIcon;
            this._default[CoverSize.Large] = largeIcon;
        }
    }
}
