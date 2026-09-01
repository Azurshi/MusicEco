namespace AudioCodec.Enum;

public enum AVDiscard: int {
    /// <summary>discard nothing</summary>
    NONE = -16,
    /// <summary>discard useless packets like 0 size packets in avi</summary>
    DEFAULT = 0,
    /// <summary>discard all non reference</summary>
    NONREF = 8,
    /// <summary>discard all bidirectional frames</summary>
    BIDIR = 16,
    /// <summary>discard all non intra frames</summary>
    NONINTRA = 24,
    /// <summary>discard all frames except keyframes</summary>
    NONKEY = 32,
    /// <summary>discard all</summary>
    ALL = 48,
}
