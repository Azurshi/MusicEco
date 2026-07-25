namespace AudioCodec.Enum;

public enum AVPixelFormat: int {
    NONE = -1,
    /// <summary>planar YUV 4:2:0, 12bpp, (1 Cr &amp; Cb sample per 2x2 Y samples)</summary>
    YUV420P = 0,
    /// <summary>packed YUV 4:2:2, 16bpp, Y0 Cb Y1 Cr</summary>
    YUYV422 = 1,
    /// <summary>packed RGB 8:8:8, 24bpp, RGBRGB...</summary>
    RGB24 = 2,
    /// <summary>packed RGB 8:8:8, 24bpp, BGRBGR...</summary>
    BGR24 = 3,
    /// <summary>planar YUV 4:2:2, 16bpp, (1 Cr &amp; Cb sample per 2x1 Y samples)</summary>
    YUV422P = 4,
    /// <summary>planar YUV 4:4:4, 24bpp, (1 Cr &amp; Cb sample per 1x1 Y samples)</summary>
    YUV444P = 5,
    /// <summary>planar YUV 4:1:0, 9bpp, (1 Cr &amp; Cb sample per 4x4 Y samples)</summary>
    YUV410P = 6,
    /// <summary>planar YUV 4:1:1, 12bpp, (1 Cr &amp; Cb sample per 4x1 Y samples)</summary>
    YUV411P = 7,
    /// <summary>Y , 8bpp</summary>
    GRAY8 = 8,
    /// <summary>Y , 1bpp, 0 is white, 1 is black, in each byte pixels are ordered from the msb to the lsb</summary>
    MONOWHITE = 9,
    /// <summary>Y , 1bpp, 0 is black, 1 is white, in each byte pixels are ordered from the msb to the lsb</summary>
    MONOBLACK = 10,
    /// <summary>8 bits with AV_PIX_FMT_RGB32 palette</summary>
    PAL8 = 11,
    /// <summary>planar YUV 4:2:0, 12bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV420P and setting color_range</summary>
    YUVJ420P = 12,
    /// <summary>planar YUV 4:2:2, 16bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV422P and setting color_range</summary>
    YUVJ422P = 13,
    /// <summary>planar YUV 4:4:4, 24bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV444P and setting color_range</summary>
    YUVJ444P = 14,
    /// <summary>packed YUV 4:2:2, 16bpp, Cb Y0 Cr Y1</summary>
    UYVY422 = 15,
    /// <summary>packed YUV 4:1:1, 12bpp, Cb Y0 Y1 Cr Y2 Y3</summary>
    UYYVYY411 = 16,
    /// <summary>packed RGB 3:3:2, 8bpp, (msb)2B 3G 3R(lsb)</summary>
    BGR8 = 17,
    /// <summary>packed RGB 1:2:1 bitstream, 4bpp, (msb)1B 2G 1R(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits</summary>
    BGR4 = 18,
    /// <summary>packed RGB 1:2:1, 8bpp, (msb)1B 2G 1R(lsb)</summary>
    BGR4_BYTE = 19,
    /// <summary>packed RGB 3:3:2, 8bpp, (msb)3R 3G 2B(lsb)</summary>
    RGB8 = 20,
    /// <summary>packed RGB 1:2:1 bitstream, 4bpp, (msb)1R 2G 1B(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits</summary>
    RGB4 = 21,
    /// <summary>packed RGB 1:2:1, 8bpp, (msb)1R 2G 1B(lsb)</summary>
    RGB4_BYTE = 22,
    /// <summary>planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 plane for the UV components, which are interleaved (first byte U and the following byte V)</summary>
    NV12 = 23,
    /// <summary>as above, but U and V bytes are swapped</summary>
    NV21 = 24,
    /// <summary>packed ARGB 8:8:8:8, 32bpp, ARGBARGB...</summary>
    ARGB = 25,
    /// <summary>packed RGBA 8:8:8:8, 32bpp, RGBARGBA...</summary>
    RGBA = 26,
    /// <summary>packed ABGR 8:8:8:8, 32bpp, ABGRABGR...</summary>
    ABGR = 27,
    /// <summary>packed BGRA 8:8:8:8, 32bpp, BGRABGRA...</summary>
    BGRA = 28,
    /// <summary>Y , 16bpp, big-endian</summary>
    GRAY16BE = 29,
    /// <summary>Y , 16bpp, little-endian</summary>
    GRAY16LE = 30,
    /// <summary>planar YUV 4:4:0 (1 Cr &amp; Cb sample per 1x2 Y samples)</summary>
    YUV440P = 31,
    /// <summary>planar YUV 4:4:0 full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV440P and setting color_range</summary>
    YUVJ440P = 32,
    /// <summary>planar YUV 4:2:0, 20bpp, (1 Cr &amp; Cb sample per 2x2 Y &amp; A samples)</summary>
    YUVA420P = 33,
    /// <summary>packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as big-endian</summary>
    RGB48BE = 34,
    /// <summary>packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as little-endian</summary>
    RGB48LE = 35,
    /// <summary>packed RGB 5:6:5, 16bpp, (msb) 5R 6G 5B(lsb), big-endian</summary>
    RGB565BE = 36,
    /// <summary>packed RGB 5:6:5, 16bpp, (msb) 5R 6G 5B(lsb), little-endian</summary>
    RGB565LE = 37,
    /// <summary>packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), big-endian , X=unused/undefined</summary>
    RGB555BE = 38,
    /// <summary>packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), little-endian, X=unused/undefined</summary>
    RGB555LE = 39,
    /// <summary>packed BGR 5:6:5, 16bpp, (msb) 5B 6G 5R(lsb), big-endian</summary>
    BGR565BE = 40,
    /// <summary>packed BGR 5:6:5, 16bpp, (msb) 5B 6G 5R(lsb), little-endian</summary>
    BGR565LE = 41,
    /// <summary>packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), big-endian , X=unused/undefined</summary>
    BGR555BE = 42,
    /// <summary>packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), little-endian, X=unused/undefined</summary>
    BGR555LE = 43,
    /// <summary>Hardware acceleration through VA-API, data[3] contains a VASurfaceID.</summary>
    VAAPI = 44,
    /// <summary>planar YUV 4:2:0, 24bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), little-endian</summary>
    YUV420P16LE = 45,
    /// <summary>planar YUV 4:2:0, 24bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), big-endian</summary>
    YUV420P16BE = 46,
    /// <summary>planar YUV 4:2:2, 32bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), little-endian</summary>
    YUV422P16LE = 47,
    /// <summary>planar YUV 4:2:2, 32bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), big-endian</summary>
    YUV422P16BE = 48,
    /// <summary>planar YUV 4:4:4, 48bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), little-endian</summary>
    YUV444P16LE = 49,
    /// <summary>planar YUV 4:4:4, 48bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), big-endian</summary>
    YUV444P16BE = 50,
    /// <summary>HW decoding through DXVA2, Picture.data[3] contains a LPDIRECT3DSURFACE9 pointer</summary>
    DXVA2_VLD = 51,
    /// <summary>packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), little-endian, X=unused/undefined</summary>
    RGB444LE = 52,
    /// <summary>packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), big-endian, X=unused/undefined</summary>
    RGB444BE = 53,
    /// <summary>packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), little-endian, X=unused/undefined</summary>
    BGR444LE = 54,
    /// <summary>packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), big-endian, X=unused/undefined</summary>
    BGR444BE = 55,
    /// <summary>8 bits gray, 8 bits alpha</summary>
    YA8 = 56,
    /// <summary>alias for AV_PIX_FMT_YA8</summary>
    Y400A = 56,
    /// <summary>alias for AV_PIX_FMT_YA8</summary>
    GRAY8A = 56,
    /// <summary>packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as big-endian</summary>
    BGR48BE = 57,
    /// <summary>packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as little-endian</summary>
    BGR48LE = 58,
    /// <summary>planar YUV 4:2:0, 13.5bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), big-endian</summary>
    YUV420P9BE = 59,
    /// <summary>planar YUV 4:2:0, 13.5bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), little-endian</summary>
    YUV420P9LE = 60,
    /// <summary>planar YUV 4:2:0, 15bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), big-endian</summary>
    YUV420P10BE = 61,
    /// <summary>planar YUV 4:2:0, 15bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), little-endian</summary>
    YUV420P10LE = 62,
    /// <summary>planar YUV 4:2:2, 20bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), big-endian</summary>
    YUV422P10BE = 63,
    /// <summary>planar YUV 4:2:2, 20bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), little-endian</summary>
    YUV422P10LE = 64,
    /// <summary>planar YUV 4:4:4, 27bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), big-endian</summary>
    YUV444P9BE = 65,
    /// <summary>planar YUV 4:4:4, 27bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), little-endian</summary>
    YUV444P9LE = 66,
    /// <summary>planar YUV 4:4:4, 30bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), big-endian</summary>
    YUV444P10BE = 67,
    /// <summary>planar YUV 4:4:4, 30bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), little-endian</summary>
    YUV444P10LE = 68,
    /// <summary>planar YUV 4:2:2, 18bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), big-endian</summary>
    YUV422P9BE = 69,
    /// <summary>planar YUV 4:2:2, 18bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), little-endian</summary>
    YUV422P9LE = 70,
    /// <summary>planar GBR 4:4:4 24bpp</summary>
    GBRP = 71,
    GBR24P = 71,
    /// <summary>planar GBR 4:4:4 27bpp, big-endian</summary>
    GBRP9BE = 72,
    /// <summary>planar GBR 4:4:4 27bpp, little-endian</summary>
    GBRP9LE = 73,
    /// <summary>planar GBR 4:4:4 30bpp, big-endian</summary>
    GBRP10BE = 74,
    /// <summary>planar GBR 4:4:4 30bpp, little-endian</summary>
    GBRP10LE = 75,
    /// <summary>planar GBR 4:4:4 48bpp, big-endian</summary>
    GBRP16BE = 76,
    /// <summary>planar GBR 4:4:4 48bpp, little-endian</summary>
    GBRP16LE = 77,
    /// <summary>planar YUV 4:2:2 24bpp, (1 Cr &amp; Cb sample per 2x1 Y &amp; A samples)</summary>
    YUVA422P = 78,
    /// <summary>planar YUV 4:4:4 32bpp, (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples)</summary>
    YUVA444P = 79,
    /// <summary>planar YUV 4:2:0 22.5bpp, (1 Cr &amp; Cb sample per 2x2 Y &amp; A samples), big-endian</summary>
    YUVA420P9BE = 80,
    /// <summary>planar YUV 4:2:0 22.5bpp, (1 Cr &amp; Cb sample per 2x2 Y &amp; A samples), little-endian</summary>
    YUVA420P9LE = 81,
    /// <summary>planar YUV 4:2:2 27bpp, (1 Cr &amp; Cb sample per 2x1 Y &amp; A samples), big-endian</summary>
    YUVA422P9BE = 82,
    /// <summary>planar YUV 4:2:2 27bpp, (1 Cr &amp; Cb sample per 2x1 Y &amp; A samples), little-endian</summary>
    YUVA422P9LE = 83,
    /// <summary>planar YUV 4:4:4 36bpp, (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples), big-endian</summary>
    YUVA444P9BE = 84,
    /// <summary>planar YUV 4:4:4 36bpp, (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples), little-endian</summary>
    YUVA444P9LE = 85,
    /// <summary>planar YUV 4:2:0 25bpp, (1 Cr &amp; Cb sample per 2x2 Y &amp; A samples, big-endian)</summary>
    YUVA420P10BE = 86,
    /// <summary>planar YUV 4:2:0 25bpp, (1 Cr &amp; Cb sample per 2x2 Y &amp; A samples, little-endian)</summary>
    YUVA420P10LE = 87,
    /// <summary>planar YUV 4:2:2 30bpp, (1 Cr &amp; Cb sample per 2x1 Y &amp; A samples, big-endian)</summary>
    YUVA422P10BE = 88,
    /// <summary>planar YUV 4:2:2 30bpp, (1 Cr &amp; Cb sample per 2x1 Y &amp; A samples, little-endian)</summary>
    YUVA422P10LE = 89,
    /// <summary>planar YUV 4:4:4 40bpp, (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples, big-endian)</summary>
    YUVA444P10BE = 90,
    /// <summary>planar YUV 4:4:4 40bpp, (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples, little-endian)</summary>
    YUVA444P10LE = 91,
    /// <summary>planar YUV 4:2:0 40bpp, (1 Cr &amp; Cb sample per 2x2 Y &amp; A samples, big-endian)</summary>
    YUVA420P16BE = 92,
    /// <summary>planar YUV 4:2:0 40bpp, (1 Cr &amp; Cb sample per 2x2 Y &amp; A samples, little-endian)</summary>
    YUVA420P16LE = 93,
    /// <summary>planar YUV 4:2:2 48bpp, (1 Cr &amp; Cb sample per 2x1 Y &amp; A samples, big-endian)</summary>
    YUVA422P16BE = 94,
    /// <summary>planar YUV 4:2:2 48bpp, (1 Cr &amp; Cb sample per 2x1 Y &amp; A samples, little-endian)</summary>
    YUVA422P16LE = 95,
    /// <summary>planar YUV 4:4:4 64bpp, (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples, big-endian)</summary>
    YUVA444P16BE = 96,
    /// <summary>planar YUV 4:4:4 64bpp, (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples, little-endian)</summary>
    YUVA444P16LE = 97,
    /// <summary>HW acceleration through VDPAU, Picture.data[3] contains a VdpVideoSurface</summary>
    VDPAU = 98,
    /// <summary>packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as little-endian, the 4 lower bits are set to 0</summary>
    XYZ12LE = 99,
    /// <summary>packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as big-endian, the 4 lower bits are set to 0</summary>
    XYZ12BE = 100,
    /// <summary>interleaved chroma YUV 4:2:2, 16bpp, (1 Cr &amp; Cb sample per 2x1 Y samples)</summary>
    NV16 = 101,
    /// <summary>interleaved chroma YUV 4:2:2, 20bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), little-endian</summary>
    NV20LE = 102,
    /// <summary>interleaved chroma YUV 4:2:2, 20bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), big-endian</summary>
    NV20BE = 103,
    /// <summary>packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian</summary>
    RGBA64BE = 104,
    /// <summary>packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian</summary>
    RGBA64LE = 105,
    /// <summary>packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian</summary>
    BGRA64BE = 106,
    /// <summary>packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian</summary>
    BGRA64LE = 107,
    /// <summary>packed YUV 4:2:2, 16bpp, Y0 Cr Y1 Cb</summary>
    YVYU422 = 108,
    /// <summary>16 bits gray, 16 bits alpha (big-endian)</summary>
    YA16BE = 109,
    /// <summary>16 bits gray, 16 bits alpha (little-endian)</summary>
    YA16LE = 110,
    /// <summary>planar GBRA 4:4:4:4 32bpp</summary>
    GBRAP = 111,
    /// <summary>planar GBRA 4:4:4:4 64bpp, big-endian</summary>
    GBRAP16BE = 112,
    /// <summary>planar GBRA 4:4:4:4 64bpp, little-endian</summary>
    GBRAP16LE = 113,
    /// <summary>HW acceleration through QSV, data[3] contains a pointer to the mfxFrameSurface1 structure.</summary>
    QSV = 114,
    /// <summary>HW acceleration though MMAL, data[3] contains a pointer to the MMAL_BUFFER_HEADER_T structure.</summary>
    MMAL = 115,
    /// <summary>HW decoding through Direct3D11 via old API, Picture.data[3] contains a ID3D11VideoDecoderOutputView pointer</summary>
    D3D11VA_VLD = 116,
    /// <summary>HW acceleration through CUDA. data[i] contain CUdeviceptr pointers exactly as for system memory frames.</summary>
    CUDA = 117,
    /// <summary>packed RGB 8:8:8, 32bpp, XRGBXRGB... X=unused/undefined</summary>
    _0RGB = 118,
    /// <summary>packed RGB 8:8:8, 32bpp, RGBXRGBX... X=unused/undefined</summary>
    RGB0 = 119,
    /// <summary>packed BGR 8:8:8, 32bpp, XBGRXBGR... X=unused/undefined</summary>
    _0BGR = 120,
    /// <summary>packed BGR 8:8:8, 32bpp, BGRXBGRX... X=unused/undefined</summary>
    BGR0 = 121,
    /// <summary>planar YUV 4:2:0,18bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), big-endian</summary>
    YUV420P12BE = 122,
    /// <summary>planar YUV 4:2:0,18bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), little-endian</summary>
    YUV420P12LE = 123,
    /// <summary>planar YUV 4:2:0,21bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), big-endian</summary>
    YUV420P14BE = 124,
    /// <summary>planar YUV 4:2:0,21bpp, (1 Cr &amp; Cb sample per 2x2 Y samples), little-endian</summary>
    YUV420P14LE = 125,
    /// <summary>planar YUV 4:2:2,24bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), big-endian</summary>
    YUV422P12BE = 126,
    /// <summary>planar YUV 4:2:2,24bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), little-endian</summary>
    YUV422P12LE = 127,
    /// <summary>planar YUV 4:2:2,28bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), big-endian</summary>
    YUV422P14BE = 128,
    /// <summary>planar YUV 4:2:2,28bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), little-endian</summary>
    YUV422P14LE = 129,
    /// <summary>planar YUV 4:4:4,36bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), big-endian</summary>
    YUV444P12BE = 130,
    /// <summary>planar YUV 4:4:4,36bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), little-endian</summary>
    YUV444P12LE = 131,
    /// <summary>planar YUV 4:4:4,42bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), big-endian</summary>
    YUV444P14BE = 132,
    /// <summary>planar YUV 4:4:4,42bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), little-endian</summary>
    YUV444P14LE = 133,
    /// <summary>planar GBR 4:4:4 36bpp, big-endian</summary>
    GBRP12BE = 134,
    /// <summary>planar GBR 4:4:4 36bpp, little-endian</summary>
    GBRP12LE = 135,
    /// <summary>planar GBR 4:4:4 42bpp, big-endian</summary>
    GBRP14BE = 136,
    /// <summary>planar GBR 4:4:4 42bpp, little-endian</summary>
    GBRP14LE = 137,
    /// <summary>planar YUV 4:1:1, 12bpp, (1 Cr &amp; Cb sample per 4x1 Y samples) full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV411P and setting color_range</summary>
    YUVJ411P = 138,
    /// <summary>bayer, BGBG..(odd line), GRGR..(even line), 8-bit samples</summary>
    BAYER_BGGR8 = 139,
    /// <summary>bayer, RGRG..(odd line), GBGB..(even line), 8-bit samples</summary>
    BAYER_RGGB8 = 140,
    /// <summary>bayer, GBGB..(odd line), RGRG..(even line), 8-bit samples</summary>
    BAYER_GBRG8 = 141,
    /// <summary>bayer, GRGR..(odd line), BGBG..(even line), 8-bit samples</summary>
    BAYER_GRBG8 = 142,
    /// <summary>bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, little-endian</summary>
    BAYER_BGGR16LE = 143,
    /// <summary>bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, big-endian</summary>
    BAYER_BGGR16BE = 144,
    /// <summary>bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, little-endian</summary>
    BAYER_RGGB16LE = 145,
    /// <summary>bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, big-endian</summary>
    BAYER_RGGB16BE = 146,
    /// <summary>bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, little-endian</summary>
    BAYER_GBRG16LE = 147,
    /// <summary>bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, big-endian</summary>
    BAYER_GBRG16BE = 148,
    /// <summary>bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, little-endian</summary>
    BAYER_GRBG16LE = 149,
    /// <summary>bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, big-endian</summary>
    BAYER_GRBG16BE = 150,
    /// <summary>planar YUV 4:4:0,20bpp, (1 Cr &amp; Cb sample per 1x2 Y samples), little-endian</summary>
    YUV440P10LE = 151,
    /// <summary>planar YUV 4:4:0,20bpp, (1 Cr &amp; Cb sample per 1x2 Y samples), big-endian</summary>
    YUV440P10BE = 152,
    /// <summary>planar YUV 4:4:0,24bpp, (1 Cr &amp; Cb sample per 1x2 Y samples), little-endian</summary>
    YUV440P12LE = 153,
    /// <summary>planar YUV 4:4:0,24bpp, (1 Cr &amp; Cb sample per 1x2 Y samples), big-endian</summary>
    YUV440P12BE = 154,
    /// <summary>packed AYUV 4:4:4,64bpp (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples), little-endian</summary>
    AYUV64LE = 155,
    /// <summary>packed AYUV 4:4:4,64bpp (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples), big-endian</summary>
    AYUV64BE = 156,
    /// <summary>hardware decoding through Videotoolbox</summary>
    VIDEOTOOLBOX = 157,
    /// <summary>like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, little-endian</summary>
    P010LE = 158,
    /// <summary>like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, big-endian</summary>
    P010BE = 159,
    /// <summary>planar GBR 4:4:4:4 48bpp, big-endian</summary>
    GBRAP12BE = 160,
    /// <summary>planar GBR 4:4:4:4 48bpp, little-endian</summary>
    GBRAP12LE = 161,
    /// <summary>planar GBR 4:4:4:4 40bpp, big-endian</summary>
    GBRAP10BE = 162,
    /// <summary>planar GBR 4:4:4:4 40bpp, little-endian</summary>
    GBRAP10LE = 163,
    /// <summary>hardware decoding through MediaCodec</summary>
    MEDIACODEC = 164,
    /// <summary>Y , 12bpp, big-endian</summary>
    GRAY12BE = 165,
    /// <summary>Y , 12bpp, little-endian</summary>
    GRAY12LE = 166,
    /// <summary>Y , 10bpp, big-endian</summary>
    GRAY10BE = 167,
    /// <summary>Y , 10bpp, little-endian</summary>
    GRAY10LE = 168,
    /// <summary>like NV12, with 16bpp per component, little-endian</summary>
    P016LE = 169,
    /// <summary>like NV12, with 16bpp per component, big-endian</summary>
    P016BE = 170,
    /// <summary>Hardware surfaces for Direct3D11.</summary>
    D3D11 = 171,
    /// <summary>Y , 9bpp, big-endian</summary>
    GRAY9BE = 172,
    /// <summary>Y , 9bpp, little-endian</summary>
    GRAY9LE = 173,
    /// <summary>IEEE-754 single precision planar GBR 4:4:4, 96bpp, big-endian</summary>
    GBRPF32BE = 174,
    /// <summary>IEEE-754 single precision planar GBR 4:4:4, 96bpp, little-endian</summary>
    GBRPF32LE = 175,
    /// <summary>IEEE-754 single precision planar GBRA 4:4:4:4, 128bpp, big-endian</summary>
    GBRAPF32BE = 176,
    /// <summary>IEEE-754 single precision planar GBRA 4:4:4:4, 128bpp, little-endian</summary>
    GBRAPF32LE = 177,
    /// <summary>DRM-managed buffers exposed through PRIME buffer sharing.</summary>
    DRM_PRIME = 178,
    /// <summary>Hardware surfaces for OpenCL.</summary>
    OPENCL = 179,
    /// <summary>Y , 14bpp, big-endian</summary>
    GRAY14BE = 180,
    /// <summary>Y , 14bpp, little-endian</summary>
    GRAY14LE = 181,
    /// <summary>IEEE-754 single precision Y, 32bpp, big-endian</summary>
    GRAYF32BE = 182,
    /// <summary>IEEE-754 single precision Y, 32bpp, little-endian</summary>
    GRAYF32LE = 183,
    /// <summary>planar YUV 4:2:2,24bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), 12b alpha, big-endian</summary>
    YUVA422P12BE = 184,
    /// <summary>planar YUV 4:2:2,24bpp, (1 Cr &amp; Cb sample per 2x1 Y samples), 12b alpha, little-endian</summary>
    YUVA422P12LE = 185,
    /// <summary>planar YUV 4:4:4,36bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), 12b alpha, big-endian</summary>
    YUVA444P12BE = 186,
    /// <summary>planar YUV 4:4:4,36bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), 12b alpha, little-endian</summary>
    YUVA444P12LE = 187,
    /// <summary>planar YUV 4:4:4, 24bpp, 1 plane for Y and 1 plane for the UV components, which are interleaved (first byte U and the following byte V)</summary>
    NV24 = 188,
    /// <summary>as above, but U and V bytes are swapped</summary>
    NV42 = 189,
    /// <summary>Vulkan hardware images.</summary>
    VULKAN = 190,
    /// <summary>packed YUV 4:2:2 like YUYV422, 20bpp, data in the high bits, big-endian</summary>
    Y210BE = 191,
    /// <summary>packed YUV 4:2:2 like YUYV422, 20bpp, data in the high bits, little-endian</summary>
    Y210LE = 192,
    /// <summary>packed RGB 10:10:10, 30bpp, (msb)2X 10R 10G 10B(lsb), little-endian, X=unused/undefined</summary>
    X2RGB10LE = 193,
    /// <summary>packed RGB 10:10:10, 30bpp, (msb)2X 10R 10G 10B(lsb), big-endian, X=unused/undefined</summary>
    X2RGB10BE = 194,
    /// <summary>packed BGR 10:10:10, 30bpp, (msb)2X 10B 10G 10R(lsb), little-endian, X=unused/undefined</summary>
    X2BGR10LE = 195,
    /// <summary>packed BGR 10:10:10, 30bpp, (msb)2X 10B 10G 10R(lsb), big-endian, X=unused/undefined</summary>
    X2BGR10BE = 196,
    /// <summary>interleaved chroma YUV 4:2:2, 20bpp, data in the high bits, big-endian</summary>
    P210BE = 197,
    /// <summary>interleaved chroma YUV 4:2:2, 20bpp, data in the high bits, little-endian</summary>
    P210LE = 198,
    /// <summary>interleaved chroma YUV 4:4:4, 30bpp, data in the high bits, big-endian</summary>
    P410BE = 199,
    /// <summary>interleaved chroma YUV 4:4:4, 30bpp, data in the high bits, little-endian</summary>
    P410LE = 200,
    /// <summary>interleaved chroma YUV 4:2:2, 32bpp, big-endian</summary>
    P216BE = 201,
    /// <summary>interleaved chroma YUV 4:2:2, 32bpp, little-endian</summary>
    P216LE = 202,
    /// <summary>interleaved chroma YUV 4:4:4, 48bpp, big-endian</summary>
    P416BE = 203,
    /// <summary>interleaved chroma YUV 4:4:4, 48bpp, little-endian</summary>
    P416LE = 204,
    /// <summary>packed VUYA 4:4:4:4, 32bpp (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples), VUYAVUYA...</summary>
    VUYA = 205,
    /// <summary>IEEE-754 half precision packed RGBA 16:16:16:16, 64bpp, RGBARGBA..., big-endian</summary>
    RGBAF16BE = 206,
    /// <summary>IEEE-754 half precision packed RGBA 16:16:16:16, 64bpp, RGBARGBA..., little-endian</summary>
    RGBAF16LE = 207,
    /// <summary>packed VUYX 4:4:4:4, 32bpp, Variant of VUYA where alpha channel is left undefined</summary>
    VUYX = 208,
    /// <summary>like NV12, with 12bpp per component, data in the high bits, zeros in the low bits, little-endian</summary>
    P012LE = 209,
    /// <summary>like NV12, with 12bpp per component, data in the high bits, zeros in the low bits, big-endian</summary>
    P012BE = 210,
    /// <summary>packed YUV 4:2:2 like YUYV422, 24bpp, data in the high bits, zeros in the low bits, big-endian</summary>
    Y212BE = 211,
    /// <summary>packed YUV 4:2:2 like YUYV422, 24bpp, data in the high bits, zeros in the low bits, little-endian</summary>
    Y212LE = 212,
    /// <summary>packed XVYU 4:4:4, 32bpp, (msb)2X 10V 10Y 10U(lsb), big-endian, variant of Y410 where alpha channel is left undefined</summary>
    XV30BE = 213,
    /// <summary>packed XVYU 4:4:4, 32bpp, (msb)2X 10V 10Y 10U(lsb), little-endian, variant of Y410 where alpha channel is left undefined</summary>
    XV30LE = 214,
    /// <summary>packed XVYU 4:4:4, 48bpp, data in the high bits, zeros in the low bits, big-endian, variant of Y412 where alpha channel is left undefined</summary>
    XV36BE = 215,
    /// <summary>packed XVYU 4:4:4, 48bpp, data in the high bits, zeros in the low bits, little-endian, variant of Y412 where alpha channel is left undefined</summary>
    XV36LE = 216,
    /// <summary>IEEE-754 single precision packed RGB 32:32:32, 96bpp, RGBRGB..., big-endian</summary>
    RGBF32BE = 217,
    /// <summary>IEEE-754 single precision packed RGB 32:32:32, 96bpp, RGBRGB..., little-endian</summary>
    RGBF32LE = 218,
    /// <summary>IEEE-754 single precision packed RGBA 32:32:32:32, 128bpp, RGBARGBA..., big-endian</summary>
    RGBAF32BE = 219,
    /// <summary>IEEE-754 single precision packed RGBA 32:32:32:32, 128bpp, RGBARGBA..., little-endian</summary>
    RGBAF32LE = 220,
    /// <summary>interleaved chroma YUV 4:2:2, 24bpp, data in the high bits, big-endian</summary>
    P212BE = 221,
    /// <summary>interleaved chroma YUV 4:2:2, 24bpp, data in the high bits, little-endian</summary>
    P212LE = 222,
    /// <summary>interleaved chroma YUV 4:4:4, 36bpp, data in the high bits, big-endian</summary>
    P412BE = 223,
    /// <summary>interleaved chroma YUV 4:4:4, 36bpp, data in the high bits, little-endian</summary>
    P412LE = 224,
    /// <summary>planar GBR 4:4:4:4 56bpp, big-endian</summary>
    GBRAP14BE = 225,
    /// <summary>planar GBR 4:4:4:4 56bpp, little-endian</summary>
    GBRAP14LE = 226,
    /// <summary>Hardware surfaces for Direct3D 12.</summary>
    D3D12 = 227,
    /// <summary>packed AYUV 4:4:4:4, 32bpp (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples), AYUVAYUV...</summary>
    AYUV = 228,
    /// <summary>packed UYVA 4:4:4:4, 32bpp (1 Cr &amp; Cb sample per 1x1 Y &amp; A samples), UYVAUYVA...</summary>
    UYVA = 229,
    /// <summary>packed VYU 4:4:4, 24bpp (1 Cr &amp; Cb sample per 1x1 Y), VYUVYU...</summary>
    VYU444 = 230,
    /// <summary>packed VYUX 4:4:4 like XV30, 32bpp, (msb)10V 10Y 10U 2X(lsb), big-endian</summary>
    V30XBE = 231,
    /// <summary>packed VYUX 4:4:4 like XV30, 32bpp, (msb)10V 10Y 10U 2X(lsb), little-endian</summary>
    V30XLE = 232,
    /// <summary>IEEE-754 half precision packed RGB 16:16:16, 48bpp, RGBRGB..., big-endian</summary>
    RGBF16BE = 233,
    /// <summary>IEEE-754 half precision packed RGB 16:16:16, 48bpp, RGBRGB..., little-endian</summary>
    RGBF16LE = 234,
    /// <summary>packed RGBA 32:32:32:32, 128bpp, RGBARGBA..., big-endian</summary>
    RGBA128BE = 235,
    /// <summary>packed RGBA 32:32:32:32, 128bpp, RGBARGBA..., little-endian</summary>
    RGBA128LE = 236,
    /// <summary>packed RGBA 32:32:32, 96bpp, RGBRGB..., big-endian</summary>
    RGB96BE = 237,
    /// <summary>packed RGBA 32:32:32, 96bpp, RGBRGB..., little-endian</summary>
    RGB96LE = 238,
    /// <summary>packed YUV 4:2:2 like YUYV422, 32bpp, big-endian</summary>
    Y216BE = 239,
    /// <summary>packed YUV 4:2:2 like YUYV422, 32bpp, little-endian</summary>
    Y216LE = 240,
    /// <summary>packed XVYU 4:4:4, 64bpp, big-endian, variant of Y416 where alpha channel is left undefined</summary>
    XV48BE = 241,
    /// <summary>packed XVYU 4:4:4, 64bpp, little-endian, variant of Y416 where alpha channel is left undefined</summary>
    XV48LE = 242,
    /// <summary>IEEE-754 half precision planer GBR 4:4:4, 48bpp, big-endian</summary>
    GBRPF16BE = 243,
    /// <summary>IEEE-754 half precision planer GBR 4:4:4, 48bpp, little-endian</summary>
    GBRPF16LE = 244,
    /// <summary>IEEE-754 half precision planar GBRA 4:4:4:4, 64bpp, big-endian</summary>
    GBRAPF16BE = 245,
    /// <summary>IEEE-754 half precision planar GBRA 4:4:4:4, 64bpp, little-endian</summary>
    GBRAPF16LE = 246,
    /// <summary>IEEE-754 half precision Y, 16bpp, big-endian</summary>
    GRAYF16BE = 247,
    /// <summary>IEEE-754 half precision Y, 16bpp, little-endian</summary>
    GRAYF16LE = 248,
    /// <summary>HW acceleration through AMF. data[0] contain AMFSurface pointer</summary>
    AMF_SURFACE = 249,
    /// <summary>Y , 32bpp, big-endian</summary>
    GRAY32BE = 250,
    /// <summary>Y , 32bpp, little-endian</summary>
    GRAY32LE = 251,
    /// <summary>IEEE-754 single precision packed YA, 32 bits gray, 32 bits alpha, 64bpp, big-endian</summary>
    YAF32BE = 252,
    /// <summary>IEEE-754 single precision packed YA, 32 bits gray, 32 bits alpha, 64bpp, little-endian</summary>
    YAF32LE = 253,
    /// <summary>IEEE-754 half precision packed YA, 16 bits gray, 16 bits alpha, 32bpp, big-endian</summary>
    YAF16BE = 254,
    /// <summary>IEEE-754 half precision packed YA, 16 bits gray, 16 bits alpha, 32bpp, little-endian</summary>
    YAF16LE = 255,
    /// <summary>planar GBRA 4:4:4:4 128bpp, big-endian</summary>
    GBRAP32BE = 256,
    /// <summary>planar GBRA 4:4:4:4 128bpp, little-endian</summary>
    GBRAP32LE = 257,
    /// <summary>planar YUV 4:4:4, 30bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), lowest bits zero, big-endian</summary>
    YUV444P10MSBBE = 258,
    /// <summary>planar YUV 4:4:4, 30bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), lowest bits zero, little-endian</summary>
    YUV444P10MSBLE = 259,
    /// <summary>planar YUV 4:4:4, 30bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), lowest bits zero, big-endian</summary>
    YUV444P12MSBBE = 260,
    /// <summary>planar YUV 4:4:4, 30bpp, (1 Cr &amp; Cb sample per 1x1 Y samples), lowest bits zero, little-endian</summary>
    YUV444P12MSBLE = 261,
    /// <summary>planar GBR 4:4:4 30bpp, lowest bits zero, big-endian</summary>
    GBRP10MSBBE = 262,
    /// <summary>planar GBR 4:4:4 30bpp, lowest bits zero, little-endian</summary>
    GBRP10MSBLE = 263,
    /// <summary>planar GBR 4:4:4 36bpp, lowest bits zero, big-endian</summary>
    GBRP12MSBBE = 264,
    /// <summary>planar GBR 4:4:4 36bpp, lowest bits zero, little-endian</summary>
    GBRP12MSBLE = 265,
    OHCODEC = 266,
    /// <summary>number of pixel formats, DO NOT USE THIS if you want to link with shared libav* because the number of formats might differ between versions</summary>
    NB = 267,
}
