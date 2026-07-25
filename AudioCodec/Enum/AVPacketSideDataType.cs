namespace AudioCodec.Enum;

public enum AVPacketSideDataType: int {
    /// <summary>An AV_PKT_DATA_PALETTE side data packet contains exactly AVPALETTE_SIZE bytes worth of palette. This side data signals that a new palette is present.</summary>
    PALETTE = 0,
    /// <summary>The AV_PKT_DATA_NEW_EXTRADATA is used to notify the codec or the format that the extradata buffer was changed and the receiving side should act upon it appropriately. The new extradata is embedded in the side data buffer and should be immediately used for processing the current frame or packet.</summary>
    NEW_EXTRADATA = 1,
    /// <summary>An AV_PKT_DATA_PARAM_CHANGE side data packet is laid out as follows:</summary>
    PARAM_CHANGE = 2,
    /// <summary>An AV_PKT_DATA_H263_MB_INFO side data packet contains a number of structures with info about macroblocks relevant to splitting the packet into smaller packets on macroblock edges (e.g. as for RFC 2190). That is, it does not necessarily contain info about all macroblocks, as long as the distance between macroblocks in the info is smaller than the target payload size. Each MB info structure is 12 bytes, and is laid out as follows:</summary>
    H263_MB_INFO = 3,
    /// <summary>This side data should be associated with an audio stream and contains ReplayGain information in form of the AVReplayGain struct.</summary>
    REPLAYGAIN = 4,
    /// <summary>This side data contains a 3x3 transformation matrix describing an affine transformation that needs to be applied to the decoded video frames for correct presentation.</summary>
    DISPLAYMATRIX = 5,
    /// <summary>This side data should be associated with a video stream and contains Stereoscopic 3D information in form of the AVStereo3D struct.</summary>
    STEREO3D = 6,
    /// <summary>This side data should be associated with an audio stream and corresponds to enum AVAudioServiceType.</summary>
    AUDIO_SERVICE_TYPE = 7,
    /// <summary>This side data contains quality related information from the encoder.</summary>
    QUALITY_STATS = 8,
    /// <summary>This side data contains an integer value representing the stream index of a &quot;fallback&quot; track. A fallback track indicates an alternate track to use when the current track can not be decoded for some reason. e.g. no decoder available for codec.</summary>
    FALLBACK_TRACK = 9,
    /// <summary>This side data corresponds to the AVCPBProperties struct.</summary>
    CPB_PROPERTIES = 10,
    /// <summary>Recommends skipping the specified number of samples</summary>
    SKIP_SAMPLES = 11,
    /// <summary>An AV_PKT_DATA_JP_DUALMONO side data packet indicates that the packet may contain &quot;dual mono&quot; audio specific to Japanese DTV and if it is true, recommends only the selected channel to be used.</summary>
    JP_DUALMONO = 12,
    /// <summary>A list of zero terminated key/value strings. There is no end marker for the list, so it is required to rely on the side data size to stop.</summary>
    STRINGS_METADATA = 13,
    /// <summary>Subtitle event position</summary>
    SUBTITLE_POSITION = 14,
    /// <summary>Data found in BlockAdditional element of matroska container. There is no end marker for the data, so it is required to rely on the side data size to recognize the end. 8 byte id (as found in BlockAddId) followed by data.</summary>
    MATROSKA_BLOCKADDITIONAL = 15,
    /// <summary>The optional first identifier line of a WebVTT cue.</summary>
    WEBVTT_IDENTIFIER = 16,
    /// <summary>The optional settings (rendering instructions) that immediately follow the timestamp specifier of a WebVTT cue.</summary>
    WEBVTT_SETTINGS = 17,
    /// <summary>A list of zero terminated key/value strings. There is no end marker for the list, so it is required to rely on the side data size to stop. This side data includes updated metadata which appeared in the stream.</summary>
    METADATA_UPDATE = 18,
    /// <summary>MPEGTS stream ID as uint8_t, this is required to pass the stream ID information from the demuxer to the corresponding muxer.</summary>
    MPEGTS_STREAM_ID = 19,
    /// <summary>Mastering display metadata (based on SMPTE-2086:2014). This metadata should be associated with a video stream and contains data in the form of the AVMasteringDisplayMetadata struct.</summary>
    MASTERING_DISPLAY_METADATA = 20,
    /// <summary>This side data should be associated with a video stream and corresponds to the AVSphericalMapping structure.</summary>
    SPHERICAL = 21,
    /// <summary>Content light level (based on CTA-861.3). This metadata should be associated with a video stream and contains data in the form of the AVContentLightMetadata struct.</summary>
    CONTENT_LIGHT_LEVEL = 22,
    /// <summary>ATSC A53 Part 4 Closed Captions. This metadata should be associated with a video stream. A53 CC bitstream is stored as uint8_t in AVPacketSideData.data. The number of bytes of CC data is AVPacketSideData.size.</summary>
    A53_CC = 23,
    /// <summary>This side data is encryption initialization data. The format is not part of ABI, use av_encryption_init_info_* methods to access.</summary>
    ENCRYPTION_INIT_INFO = 24,
    /// <summary>This side data contains encryption info for how to decrypt the packet. The format is not part of ABI, use av_encryption_info_* methods to access.</summary>
    ENCRYPTION_INFO = 25,
    /// <summary>Active Format Description data consisting of a single byte as specified in ETSI TS 101 154 using AVActiveFormatDescription enum.</summary>
    AFD = 26,
    /// <summary>Producer Reference Time data corresponding to the AVProducerReferenceTime struct, usually exported by some encoders (on demand through the prft flag set in the AVCodecContext export_side_data field).</summary>
    PRFT = 27,
    /// <summary>ICC profile data consisting of an opaque octet buffer following the format described by ISO 15076-1.</summary>
    ICC_PROFILE = 28,
    /// <summary>DOVI configuration ref: dolby-vision-bitstreams-within-the-iso-base-media-file-format-v2.1.2, section 2.2 dolby-vision-bitstreams-in-mpeg-2-transport-stream-multiplex-v1.2, section 3.3 Tags are stored in struct AVDOVIDecoderConfigurationRecord.</summary>
    DOVI_CONF = 29,
    /// <summary>Timecode which conforms to SMPTE ST 12-1:2014. The data is an array of 4 uint32_t where the first uint32_t describes how many (1-3) of the other timecodes are used. The timecode format is described in the documentation of av_timecode_get_smpte_from_framenum() function in libavutil/timecode.h.</summary>
    S12M_TIMECODE = 30,
    /// <summary>HDR10+ dynamic metadata associated with a video frame. The metadata is in the form of the AVDynamicHDRPlus struct and contains information for color volume transform - application 4 of SMPTE 2094-40:2016 standard.</summary>
    DYNAMIC_HDR10_PLUS = 31,
    /// <summary>IAMF Mix Gain Parameter Data associated with the audio frame. This metadata is in the form of the AVIAMFParamDefinition struct and contains information defined in sections 3.6.1 and 3.8.1 of the Immersive Audio Model and Formats standard.</summary>
    IAMF_MIX_GAIN_PARAM = 32,
    /// <summary>IAMF Demixing Info Parameter Data associated with the audio frame. This metadata is in the form of the AVIAMFParamDefinition struct and contains information defined in sections 3.6.1 and 3.8.2 of the Immersive Audio Model and Formats standard.</summary>
    IAMF_DEMIXING_INFO_PARAM = 33,
    /// <summary>IAMF Recon Gain Info Parameter Data associated with the audio frame. This metadata is in the form of the AVIAMFParamDefinition struct and contains information defined in sections 3.6.1 and 3.8.3 of the Immersive Audio Model and Formats standard.</summary>
    IAMF_RECON_GAIN_INFO_PARAM = 34,
    /// <summary>Ambient viewing environment metadata, as defined by H.274. This metadata should be associated with a video stream and contains data in the form of the AVAmbientViewingEnvironment struct.</summary>
    AMBIENT_VIEWING_ENVIRONMENT = 35,
    /// <summary>The number of pixels to discard from the top/bottom/left/right border of the decoded frame to obtain the sub-rectangle intended for presentation.</summary>
    FRAME_CROPPING = 36,
    /// <summary>Raw LCEVC payload data, as a uint8_t array, with NAL emulation bytes intact.</summary>
    LCEVC = 37,
    /// <summary>This side data contains information about the reference display width(s) and reference viewing distance(s) as well as information about the corresponding reference stereo pair(s), i.e., the pair(s) of views to be displayed for the viewer&apos;s left and right eyes on the reference display at the reference viewing distance. The payload is the AV3DReferenceDisplaysInfo struct defined in libavutil/tdrdi.h.</summary>
    _3D_REFERENCE_DISPLAYS = 38,
    /// <summary>Contains the last received RTCP SR (Sender Report) information in the form of the AVRTCPSenderReport struct.</summary>
    RTCP_SR = 39,
    /// <summary>Extensible image file format metadata. The payload is a buffer containing EXIF metadata, starting with either 49 49 2a 00, or 4d 4d 00 2a.</summary>
    EXIF = 40,
    /// <summary>The number of side data types. This is not part of the public API/ABI in the sense that it may change when new side data types are added. This must stay the last enum value. If its value becomes huge, some code using it needs to be updated as it assumes it to be smaller than other limits.</summary>
    NB = 41,
}
