namespace AudioCodec.Enum;

public enum AVFrameSideDataType: int {
    /// <summary>The data is the AVPanScan struct defined in libavcodec.</summary>
    PANSCAN = 0,
    /// <summary>ATSC A53 Part 4 Closed Captions. A53 CC bitstream is stored as uint8_t in AVFrameSideData.data. The number of bytes of CC data is AVFrameSideData.size.</summary>
    A53_CC = 1,
    /// <summary>Stereoscopic 3d metadata. The data is the AVStereo3D struct defined in libavutil/stereo3d.h.</summary>
    STEREO3D = 2,
    /// <summary>The data is the AVMatrixEncoding enum defined in libavutil/channel_layout.h.</summary>
    MATRIXENCODING = 3,
    /// <summary>Metadata relevant to a downmix procedure. The data is the AVDownmixInfo struct defined in libavutil/downmix_info.h.</summary>
    DOWNMIX_INFO = 4,
    /// <summary>ReplayGain information in the form of the AVReplayGain struct.</summary>
    REPLAYGAIN = 5,
    /// <summary>This side data contains a 3x3 transformation matrix describing an affine transformation that needs to be applied to the frame for correct presentation.</summary>
    DISPLAYMATRIX = 6,
    /// <summary>Active Format Description data consisting of a single byte as specified in ETSI TS 101 154 using AVActiveFormatDescription enum.</summary>
    AFD = 7,
    /// <summary>Motion vectors exported by some codecs (on demand through the export_mvs flag set in the libavcodec AVCodecContext flags2 option). The data is the AVMotionVector struct defined in libavutil/motion_vector.h.</summary>
    MOTION_VECTORS = 8,
    /// <summary>Recommends skipping the specified number of samples. This is exported only if the &quot;skip_manual&quot; AVOption is set in libavcodec. This has the same format as AV_PKT_DATA_SKIP_SAMPLES.</summary>
    SKIP_SAMPLES = 9,
    /// <summary>This side data must be associated with an audio frame and corresponds to enum AVAudioServiceType defined in avcodec.h.</summary>
    AUDIO_SERVICE_TYPE = 10,
    /// <summary>Mastering display metadata associated with a video frame. The payload is an AVMasteringDisplayMetadata type and contains information about the mastering display color volume.</summary>
    MASTERING_DISPLAY_METADATA = 11,
    /// <summary>The GOP timecode in 25 bit timecode format. Data format is 64-bit integer. This is set on the first frame of a GOP that has a temporal reference of 0.</summary>
    GOP_TIMECODE = 12,
    /// <summary>The data represents the AVSphericalMapping structure defined in libavutil/spherical.h.</summary>
    SPHERICAL = 13,
    /// <summary>Content light level (based on CTA-861.3). This payload contains data in the form of the AVContentLightMetadata struct.</summary>
    CONTENT_LIGHT_LEVEL = 14,
    /// <summary>The data contains an ICC profile as an opaque octet buffer following the format described by ISO 15076-1 with an optional name defined in the metadata key entry &quot;name&quot;.</summary>
    ICC_PROFILE = 15,
    /// <summary>Timecode which conforms to SMPTE ST 12-1. The data is an array of 4 uint32_t where the first uint32_t describes how many (1-3) of the other timecodes are used. The timecode format is described in the documentation of av_timecode_get_smpte_from_framenum() function in libavutil/timecode.h.</summary>
    S12M_TIMECODE = 16,
    /// <summary>HDR dynamic metadata associated with a video frame. The payload is an AVDynamicHDRPlus type and contains information for color volume transform - application 4 of SMPTE 2094-40:2016 standard.</summary>
    DYNAMIC_HDR_PLUS = 17,
    /// <summary>Regions Of Interest, the data is an array of AVRegionOfInterest type, the number of array element is implied by AVFrameSideData.size / AVRegionOfInterest.self_size.</summary>
    REGIONS_OF_INTEREST = 18,
    /// <summary>Encoding parameters for a video frame, as described by AVVideoEncParams.</summary>
    VIDEO_ENC_PARAMS = 19,
    /// <summary>User data unregistered metadata associated with a video frame. This is the H.26[45] UDU SEI message, and shouldn&apos;t be used for any other purpose The data is stored as uint8_t in AVFrameSideData.data which is 16 bytes of uuid_iso_iec_11578 followed by AVFrameSideData.size - 16 bytes of user_data_payload_byte.</summary>
    SEI_UNREGISTERED = 20,
    /// <summary>Film grain parameters for a frame, described by AVFilmGrainParams. Must be present for every frame which should have film grain applied.</summary>
    FILM_GRAIN_PARAMS = 21,
    /// <summary>Bounding boxes for object detection and classification, as described by AVDetectionBBoxHeader.</summary>
    DETECTION_BBOXES = 22,
    /// <summary>Dolby Vision RPU raw data, suitable for passing to x265 or other libraries. Array of uint8_t, with NAL emulation bytes intact.</summary>
    DOVI_RPU_BUFFER = 23,
    /// <summary>Parsed Dolby Vision metadata, suitable for passing to a software implementation. The payload is the AVDOVIMetadata struct defined in libavutil/dovi_meta.h.</summary>
    DOVI_METADATA = 24,
    /// <summary>HDR Vivid dynamic metadata associated with a video frame. The payload is an AVDynamicHDRVivid type and contains information for color volume transform - CUVA 005.1-2021.</summary>
    DYNAMIC_HDR_VIVID = 25,
    /// <summary>Ambient viewing environment metadata, as defined by H.274.</summary>
    AMBIENT_VIEWING_ENVIRONMENT = 26,
    /// <summary>Provide encoder-specific hinting information about changed/unchanged portions of a frame. It can be used to pass information about which macroblocks can be skipped because they didn&apos;t change from the corresponding ones in the previous frame. This could be useful for applications which know this information in advance to speed up encoding.</summary>
    VIDEO_HINT = 27,
    /// <summary>Raw LCEVC payload data, as a uint8_t array, with NAL emulation bytes intact.</summary>
    LCEVC = 28,
    /// <summary>This side data must be associated with a video frame. The presence of this side data indicates that the video stream is composed of multiple views (e.g. stereoscopic 3D content, cf. H.264 Annex H or H.265 Annex G). The data is an int storing the view ID.</summary>
    VIEW_ID = 29,
    /// <summary>This side data contains information about the reference display width(s) and reference viewing distance(s) as well as information about the corresponding reference stereo pair(s), i.e., the pair(s) of views to be displayed for the viewer&apos;s left and right eyes on the reference display at the reference viewing distance. The payload is the AV3DReferenceDisplaysInfo struct defined in libavutil/tdrdi.h.</summary>
    _3D_REFERENCE_DISPLAYS = 30,
    /// <summary>Extensible image file format metadata. The payload is a buffer containing EXIF metadata, starting with either 49 49 2a 00, or 4d 4d 00 2a.</summary>
    EXIF = 31,
}
