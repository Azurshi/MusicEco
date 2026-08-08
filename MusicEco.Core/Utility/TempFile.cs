using System.Diagnostics;
#if ANDROID
using MusicEco.Core.Platforms.Android;
using Uri = Android.Net.Uri;
#endif
namespace MusicEco.Core.Utility;

public sealed class TempFile {
    private readonly string _path;
    public long Length { get; private set; }
#if ANDROID
    public const int BufferSize = 1024 * 1024;
#endif
    public TempFile() {
#if WINDOWS
        this._path = Path.Combine(
            Path.GetTempPath(),
            Config.ProjectName,
            Guid.NewGuid().ToString("N"));
#elif ANDROID
        this._path = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString("N"));
#else
        throw new NotImplementedException();
#endif
        this.Length = 0;
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
#if WINDOWS || ANDROID
        File.WriteAllBytes(this._path, data);
#endif
        Length = data.Length;
    }
    public void Write(ReadOnlyMemory<byte> data) {
        this.Write(data.Span);
    }
    public long ReadAndDispose(byte[] buffer) {
#if WINDOWS || ANDROID
        using(var stream = File.OpenRead(this._path)) {
#else
        using(var stream = File.OpenRead(this._path)) {
#endif
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
