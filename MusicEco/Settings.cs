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
}