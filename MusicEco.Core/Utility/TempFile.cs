using System.Diagnostics;

namespace MusicEco.Core.Utility;

public sealed class TempFile {
    private readonly string _path;
    public long Length { get; private set; }
    public TempFile() {
        _path = Path.Combine(
            Path.GetTempPath(),
            Config.ProjectName,
            Guid.NewGuid().ToString("N"));
        Length = 0;
    }
    public async Task WriteAsync(byte[] data) {
        await File.WriteAllBytesAsync(_path, data);
        Length = data.LongLength;
    }
    public async Task<byte[]> ReadAndDisposeAsync() {
        var result = await File.ReadAllBytesAsync(_path);
        Dispose();
        return result;
    }
    public void Write(ReadOnlySpan<byte> data) {
        File.WriteAllBytes(this._path, data);
        Length = data.Length;
    }
    public void Write(ReadOnlyMemory<byte> data) {
        this.Write(data.Span);
    }
    public long ReadAndDispose(byte[] buffer) {
        using(var stream = File.OpenRead(this._path)) {
            stream.ReadExactly(buffer.AsSpan(0, (int)stream.Length));
            Debug.Assert(stream.Position == Length);
        }
        Dispose();
        return Length;
    }
    public void Dispose() {
        if (File.Exists(_path)) {
            File.Delete(_path);
        }
    }
}
