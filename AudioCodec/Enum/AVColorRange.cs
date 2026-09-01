namespace AudioCodec.Enum;

public enum AVColorRange: int {
    UNSPECIFIED = 0,
    /// <summary>Narrow or limited range content.</summary>
    MPEG = 1,
    /// <summary>Full range content.</summary>
    JPEG = 2,
    /// <summary>Not part of ABI</summary>
    NB = 3,
}
