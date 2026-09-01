#if WINDOWS
using MusicEco.Core.Utility;
using System.Diagnostics;

namespace MusicEco.Platform;

public class TempFile: ITempFile {
    private readonly string _path;
    public long Length { get; private set; }
    public TempFile() {
        this._path = Path.Combine(
            Path.GetTempPath(),
            MusicEco.Core.Config.ProjectName,
            Guid.NewGuid().ToString("N"));
    }
    public void Dispose() {
        if (File.Exists(this._path)) {
            File.Delete(this._path);
        }
    }

    public long ReadAndDispose(byte[] output) {
        using (var stream = File.OpenRead(this._path)) {
            stream.ReadExactly(output.AsSpan(0, (int)stream.Length));
            Debug.Assert(stream.Position == Length);
        }
        Dispose();
        return Length;
    }

    public void Write(ReadOnlySpan<byte> data) {
        Length = data.Length;
        File.WriteAllBytes(this._path, data);
    }

    public async Task WriteAsync(ReadOnlyMemory<byte> data) {
        Length = data.Length;
        await File.WriteAllBytesAsync(this._path, data);
    }
}
#endif