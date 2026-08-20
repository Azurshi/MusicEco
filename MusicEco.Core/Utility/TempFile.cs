namespace MusicEco.Core.Utility;

public interface ITempFile {
    public long Length { get; }
    public Task WriteAsync(ReadOnlyMemory<byte> data);
    public void Write(ReadOnlySpan<byte> data);
    public long ReadAndDispose(byte[] output);
    public void Dispose();
}
