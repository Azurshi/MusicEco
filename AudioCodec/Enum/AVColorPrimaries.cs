namespace AudioCodec.Enum;

public enum AVColorPrimaries: int {
    RESERVED0 = 0,
    /// <summary>also ITU-R BT1361 / IEC 61966-2-4 / SMPTE RP 177 Annex B</summary>
    BT709 = 1,
    UNSPECIFIED = 2,
    RESERVED = 3,
    /// <summary>also FCC Title 47 Code of Federal Regulations 73.682 (a)(20)</summary>
    BT470M = 4,
    /// <summary>also ITU-R BT601-6 625 / ITU-R BT1358 625 / ITU-R BT1700 625 PAL &amp; SECAM</summary>
    BT470BG = 5,
    /// <summary>also ITU-R BT601-6 525 / ITU-R BT1358 525 / ITU-R BT1700 NTSC</summary>
    SMPTE170M = 6,
    /// <summary>identical to above, also called &quot;SMPTE C&quot; even though it uses D65</summary>
    SMPTE240M = 7,
    /// <summary>colour filters using Illuminant C</summary>
    FILM = 8,
    /// <summary>ITU-R BT2020</summary>
    BT2020 = 9,
    /// <summary>SMPTE ST 428-1 (CIE 1931 XYZ)</summary>
    SMPTE428 = 10,
    SMPTEST428_1 = 10,
    /// <summary>SMPTE ST 431-2 (2011) / DCI P3</summary>
    SMPTE431 = 11,
    /// <summary>SMPTE ST 432-1 (2010) / P3 D65 / Display P3</summary>
    SMPTE432 = 12,
    /// <summary>EBU Tech. 3213-E (nothing there) / one of JEDEC P22 group phosphors</summary>
    EBU3213 = 22,
    JEDEC_P22 = 22,
    /// <summary>Not part of ABI</summary>
    NB = 23,
    EXT_BASE = 256,
    V_GAMUT = 256,
    /// <summary>Not part of ABI</summary>
    EXT_NB = 257,
}
