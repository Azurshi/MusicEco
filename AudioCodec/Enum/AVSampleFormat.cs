namespace AudioCodec.Enum;

public enum AVSampleFormat: int {
    NONE = -1,
    /// <summary>unsigned 8 bits</summary>
    U8 = 0,
    /// <summary>signed 16 bits</summary>
    S16 = 1,
    /// <summary>signed 32 bits</summary>
    S32 = 2,
    /// <summary>float</summary>
    FLT = 3,
    /// <summary>double</summary>
    DBL = 4,
    /// <summary>unsigned 8 bits, planar</summary>
    U8P = 5,
    /// <summary>signed 16 bits, planar</summary>
    S16P = 6,
    /// <summary>signed 32 bits, planar</summary>
    S32P = 7,
    /// <summary>float, planar</summary>
    FLTP = 8,
    /// <summary>double, planar</summary>
    DBLP = 9,
    /// <summary>signed 64 bits</summary>
    S64 = 10,
    /// <summary>signed 64 bits, planar</summary>
    S64P = 11,
    /// <summary>Number of sample formats. DO NOT USE if linking dynamically</summary>
    NB = 12,
}
