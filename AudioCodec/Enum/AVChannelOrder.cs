namespace AudioCodec.Enum;

public enum AVChannelOrder: int {
    AV_CHANNEL_ORDER_UNSPEC = 0,
    AV_CHANNEL_ORDER_NATIVE = 1,
    AV_CHANNEL_ORDER_CUSTOM = 2,
    AV_CHANNEL_ORDER_AMBISONIC = 3,
    FF_CHANNEL_ORDER_NB = 4
}
