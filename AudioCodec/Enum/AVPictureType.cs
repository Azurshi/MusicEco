namespace AudioCodec.Enum;

public enum AVPictureType: int {
    /// <summary>Undefined</summary>
    NONE = 0,
    /// <summary>Intra</summary>
    I = 1,
    /// <summary>Predicted</summary>
    P = 2,
    /// <summary>Bi-dir predicted</summary>
    B = 3,
    /// <summary>S(GMC)-VOP MPEG-4</summary>
    S = 4,
    /// <summary>Switching Intra</summary>
    SI = 5,
    /// <summary>Switching Predicted</summary>
    SP = 6,
    /// <summary>BI type</summary>
    BI = 7,
}
