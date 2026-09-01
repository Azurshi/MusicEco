using MusicEco.Core.Services;
using MusicEco.Image.Decoder;
using MusicEco.Image.Encoder;

namespace MusicEco.Image;

public static class ServiceRegister {
    public static IServiceCollection RegisterImage(this IServiceCollection services) {
        services.AddTransient<IImageEncoder, SkiaImageEncoder>();
        services.AddTransient<IImageDecoder, SkiaImageDecoder>();
        services.AddTransient<IIconEncoder, SkiaIconEncoder>();
        services.AddTransient<IIconDecoder, SkiaIconDecoder>();
        return services;
    }
}
