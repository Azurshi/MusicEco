namespace AudioCodec;

public static class Config {
#if WINDOWS
    public const string AVFormat = "avformat-63";
    public const string AVCodec = "avcodec-63";
    public const string AVUtil = "avutil-61";
    public const string AVResample = "swresample-7";
#elif ANDROID
    public const string AVFormat = "libavformat";
    public const string AVCodec = "libavcodec";
    public const string AVUtil = "libavutil";
    public const string AVResample = "libswresample";
#else
    public const string AVFormat = "ukn";
    public const string AVCodec = "ukn";
    public const string AVUtil = "ukn";
    public const string AVResample = "ukn";
#endif
}
