namespace AudioPlayer;

// All the code in this file is only included on Mac Catalyst.
#if MACCATALYST
public partial class AudioPlayer {
    public partial void Play(Stream stream) {
        throw new NotImplementedException();
    }
    public partial void Seek(TimeSpan position) {
        throw new NotImplementedException();
    }
    public partial void Pause() {
        throw new NotImplementedException();
    }
    public partial void Resume() {
        throw new NotImplementedException();
    }
    public partial TimeSpan GetDuration() {
        throw new NotImplementedException();
    }
    public partial TimeSpan GetPosition() {
        throw new NotImplementedException();
    }
    public partial TimeSpan GetDecodedPosition() {
        throw new NotImplementedException();
    }
    public partial void Dispose() {
        throw new NotImplementedException();
    }
    public partial float GetVolume() {
        throw new NotImplementedException();
    }
    public partial void SetVolume(float volume) {
        throw new NotImplementedException();
    }
    public partial PlaybackState GetState() {
        throw new NotImplementedException();
    }
}
#endif
