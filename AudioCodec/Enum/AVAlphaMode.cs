namespace AudioCodec.Enum;

public enum AVAlphaMode: int {
    /// <summary>Unknown alpha handling, or no alpha channel</summary>
    UNSPECIFIED = 0,
    /// <summary>Alpha channel is multiplied into color values</summary>
    PREMULTIPLIED = 1,
    /// <summary>Alpha channel is independent of color values</summary>
    STRAIGHT = 2,
    /// <summary>Not part of ABI</summary>
    NB = 3,
}
