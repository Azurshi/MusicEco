namespace AudioCodec.Enum;

public enum AVDurationEstimationMethod: int {
    /// <summary>Duration accurately estimated from PTSes</summary>
    FROM_PTS = 0,
    /// <summary>Duration estimated from a stream with a known duration</summary>
    FROM_STREAM = 1,
    /// <summary>Duration estimated from bitrate (less accurate)</summary>
    FROM_BITRATE = 2,
}
