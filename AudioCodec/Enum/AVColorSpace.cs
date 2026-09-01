namespace AudioCodec.Enum;

public enum AVColorSpace: int {
    /// <summary>order of coefficients is actually GBR, also IEC 61966-2-1 (sRGB), YZX and ST 428-1</summary>
    RGB = 0,
    /// <summary>also ITU-R BT1361 / IEC 61966-2-4 xvYCC709 / derived in SMPTE RP 177 Annex B</summary>
    BT709 = 1,
    UNSPECIFIED = 2,
    /// <summary>reserved for future use by ITU-T and ISO/IEC just like 15-255 are</summary>
    RESERVED = 3,
    /// <summary>FCC Title 47 Code of Federal Regulations 73.682 (a)(20)</summary>
    FCC = 4,
    /// <summary>also ITU-R BT601-6 625 / ITU-R BT1358 625 / ITU-R BT1700 625 PAL &amp; SECAM / IEC 61966-2-4 xvYCC601</summary>
    BT470BG = 5,
    /// <summary>also ITU-R BT601-6 525 / ITU-R BT1358 525 / ITU-R BT1700 NTSC / functionally identical to above</summary>
    SMPTE170M = 6,
    /// <summary>derived from 170M primaries and D65 white point, 170M is derived from BT470 System M&apos;s primaries</summary>
    SMPTE240M = 7,
    /// <summary>used by Dirac / VC-2 and H.264 FRext, see ITU-T SG16</summary>
    YCGCO = 8,
    YCOCG = 8,
    /// <summary>ITU-R BT2020 non-constant luminance system</summary>
    BT2020_NCL = 9,
    /// <summary>ITU-R BT2020 constant luminance system</summary>
    BT2020_CL = 10,
    /// <summary>SMPTE 2085, Y&apos;D&apos;zD&apos;x</summary>
    SMPTE2085 = 11,
    /// <summary>Chromaticity-derived non-constant luminance system</summary>
    CHROMA_DERIVED_NCL = 12,
    /// <summary>Chromaticity-derived constant luminance system</summary>
    CHROMA_DERIVED_CL = 13,
    /// <summary>ITU-R BT.2100-0, ICtCp</summary>
    ICTCP = 14,
    /// <summary>SMPTE ST 2128, IPT-C2</summary>
    IPT_C2 = 15,
    /// <summary>YCgCo-R, even addition of bits</summary>
    YCGCO_RE = 16,
    /// <summary>YCgCo-R, odd addition of bits</summary>
    YCGCO_RO = 17,
    /// <summary>Not part of ABI</summary>
    NB = 18,
}
