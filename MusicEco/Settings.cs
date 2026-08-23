using SkiaSharp;
using System.Numerics;

namespace MusicEco;

public static class SettingFields {
    public const string IconDecoderNumWorkers = nameof(IconDecoderNumWorkers);
    public const string IconDecoderCapacity = nameof(IconDecoderCapacity);
    public const string AudioPlayerFPS = nameof(AudioPlayerFPS);
    public const string PerSeekSeconds = nameof(PerSeekSeconds);
}

public static class Config {
    public const int MinNameLength = 3;
    public const float MinPlayedRatio = 0.1f;
    public static readonly TimeSpan UserInputDelay = TimeSpan.FromMilliseconds(300);
    public static readonly TimeSpan AppLoopDelta = TimeSpan.FromMilliseconds(100);
    public static readonly Vector2 MaxIconButtonSize = new(64, 64);
}
public static class SamplingOptions {
    public static readonly SKSamplingOptions None = SKSamplingOptions.Default;
    public static readonly SKSamplingOptions Nearest = new(SKFilterMode.Nearest);
    public static readonly SKSamplingOptions Bilinear = new(SKFilterMode.Linear);
    /// <summary>
    /// Good for scaling down significant large image.
    /// </summary>
    public static readonly SKSamplingOptions Trilinear = new(SKFilterMode.Linear, SKMipmapMode.Linear);
    public static readonly SKSamplingOptions Cubic = new(SKCubicResampler.Mitchell);
}
