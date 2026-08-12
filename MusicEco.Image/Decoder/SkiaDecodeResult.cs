using MusicEco.Core.Services;
using SkiaSharp;

namespace MusicEco.Image.Decoder;

public class SkiaDecodeResult: IDecodeResult {
    public bool Success => true;
    public SKImage Image { get; init; }
    internal SkiaDecodeResult(SKImage image) {
        this.Image = image;
    }
}
public class FailedDecodeResult: IDecodeResult {
    public bool Success => false;
}
