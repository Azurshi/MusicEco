namespace AudioCodec.Enum;

public enum AVChromaLocation: int {
    UNSPECIFIED = 0,
    /// <summary>MPEG-2/4 4:2:0, H.264 default for 4:2:0</summary>
    LEFT = 1,
    /// <summary>MPEG-1 4:2:0, JPEG 4:2:0, H.263 4:2:0</summary>
    CENTER = 2,
    /// <summary>ITU-R 601, SMPTE 274M 296M S314M(DV 4:1:1), mpeg2 4:2:2</summary>
    TOPLEFT = 3,
    TOP = 4,
    BOTTOMLEFT = 5,
    BOTTOM = 6,
    /// <summary>Not part of ABI</summary>
    NB = 7,
}
