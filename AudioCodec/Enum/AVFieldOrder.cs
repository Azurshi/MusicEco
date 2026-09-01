namespace AudioCodec.Enum;

public enum AVFieldOrder: int {
    UNKNOWN = 0,
    PROGRESSIVE = 1,
    /// <summary>Top coded_first, top displayed first</summary>
    TT = 2,
    /// <summary>Bottom coded first, bottom displayed first</summary>
    BB = 3,
    /// <summary>Top coded first, bottom displayed first</summary>
    TB = 4,
    /// <summary>Bottom coded first, top displayed first</summary>
    BT = 5,
}
