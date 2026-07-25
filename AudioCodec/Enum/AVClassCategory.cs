namespace AudioCodec.Enum;

public enum AVClassCategory: int {
    NA = 0,
    INPUT = 1,
    OUTPUT = 2,
    MUXER = 3,
    DEMUXER = 4,
    ENCODER = 5,
    DECODER = 6,
    FILTER = 7,
    BITSTREAM_FILTER = 8,
    SWSCALER = 9,
    HWDEVICE= 10,
    DEVICE_VIDEO_OUTPUT = 40,
    DEVICE_VIDEO_INPUT = 41,
    DEVICE_AUDIO_OUTPUT = 42,
    DEVICE_AUDIO_INPUT = 43,
    DEVICE_OUTPUT = 44,
    DEVICE_INPUT = 45,
    NB = 46
}
