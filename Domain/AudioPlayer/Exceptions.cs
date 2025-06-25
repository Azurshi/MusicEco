using Domain.Exceptions;

namespace Domain.AudioPlayer;
public class FailedToAccessFileException : BaseException {
    public string FilePath { get; set; }
    public FailedToAccessFileException(string filePath) {
        this.FilePath = filePath;
        this.Type = "File error";
        this.Info = $"Failed to access file: {filePath}";
    }
}
public class AudioPlayerError : BaseException {

}