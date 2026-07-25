namespace AudioCodec.FFmpeg;

public static class Flags {
    public const int SEEK_SET = 0;
    public const int SEEK_CUR = 1;
    public const int SEEK_END = 2;
    public const int AVSEEK_SIZE = 0x10000;
    public const int AVERR_INVALIDDATA = -1094995529;
    public const int AVERR_EIO = -5;
    public const int AVERR_EOF = -541478725;
    public const int AVERR_EAGAIN = -11; // Platform specific value
    public const int AVFMT_FLAG_CUSTOM_IO = 0x0080;
}
