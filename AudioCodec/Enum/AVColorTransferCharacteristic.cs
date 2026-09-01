namespace AudioCodec.Enum;

public enum AVColorTransferCharacteristic: int {
    RESERVED0 = 0,
    /// <summary>also ITU-R BT1361</summary>
    BT709 = 1,
    UNSPECIFIED = 2,
    RESERVED = 3,
    /// <summary>also ITU-R BT470M / ITU-R BT1700 625 PAL &amp; SECAM</summary>
    GAMMA22 = 4,
    /// <summary>also ITU-R BT470BG</summary>
    GAMMA28 = 5,
    /// <summary>also ITU-R BT601-6 525 or 625 / ITU-R BT1358 525 or 625 / ITU-R BT1700 NTSC</summary>
    SMPTE170M = 6,
    SMPTE240M = 7,
    /// <summary>&quot;Linear transfer characteristics&quot;</summary>
    LINEAR = 8,
    /// <summary>&quot;Logarithmic transfer characteristic (100:1 range)&quot;</summary>
    LOG = 9,
    /// <summary>&quot;Logarithmic transfer characteristic (100 * Sqrt(10) : 1 range)&quot;</summary>
    LOG_SQRT = 10,
    /// <summary>IEC 61966-2-4</summary>
    IEC61966_2_4 = 11,
    /// <summary>ITU-R BT1361 Extended Colour Gamut</summary>
    BT1361_ECG = 12,
    /// <summary>IEC 61966-2-1 (sRGB or sYCC)</summary>
    IEC61966_2_1 = 13,
    /// <summary>ITU-R BT2020 for 10-bit system</summary>
    BT2020_10 = 14,
    /// <summary>ITU-R BT2020 for 12-bit system</summary>
    BT2020_12 = 15,
    /// <summary>SMPTE ST 2084 for 10-, 12-, 14- and 16-bit systems</summary>
    SMPTE2084 = 16,
    SMPTEST2084 = 16,
    /// <summary>SMPTE ST 428-1</summary>
    SMPTE428 = 17,
    SMPTEST428_1 = 17,
    /// <summary>ARIB STD-B67, known as &quot;Hybrid log-gamma&quot;</summary>
    ARIB_STD_B67 = 18,
    /// <summary>Not part of ABI</summary>
    NB = 19,
    EXT_BASE = 256,
    V_LOG = 256,
    /// <summary>Not part of ABI</summary>
    EXT_NB = 257,
}
