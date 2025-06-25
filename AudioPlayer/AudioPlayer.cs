namespace AudioPlayer;

// All the code in this file is included in all platforms.
#if WINDOWS
public class AudioPlayer: WindowsAudioPlayer { }
#elif ANDROID
public class AudioPlayer: AndroidAudioPlayer { }
#endif
