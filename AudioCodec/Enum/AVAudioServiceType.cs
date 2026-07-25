namespace AudioCodec.Enum;

public enum AVAudioServiceType: int {
    MAIN = 0,
    EFFECTS = 1,
    VISUALLY_IMPAIRED = 2,
    HEARING_IMPAIRED = 3,
    DIALOGUE = 4,
    COMMENTARY = 5,
    EMERGENCY = 6,
    VOICE_OVER = 7,
    KARAOKE = 8,
    /// <summary>Not part of ABI</summary>
    NB = 9,
}
