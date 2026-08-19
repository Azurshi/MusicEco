#if ANDROID || WINDOWS

using Blake3;
using MusicEco.Core.Types;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MusicEco.Data.Services;

internal unsafe class NativeBlakeWrapper {
    private const string LibraryName = "music_eco_blake3";
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "music_eco_blake3_create")]
    public static extern nint Create();
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "music_eco_blake3_init")]
    public static extern int Init(nint context);
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "music_eco_blake3_update")]
    public static extern int Update(
        nint context, byte* input, nuint inputLength);
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "music_eco_blake3_finalize")]
    public static extern int Finalize(
        nint context, byte* output, nuint outputLength);
    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "music_eco_blake3_dispose")]
    public static extern void Dispose(nint context);
}

internal partial class Scanner {
    private static long ReadTicks = 0;
    private static long HashTicks = 0;
    private static long FinalizeTicks = 0;
    private static Hash256 ComputeHash(Memory<byte> data) {
        return ComputeHashFast(data.Span);
        //Span<byte> output = stackalloc byte[32];
        //Hasher.Hash(data.Span, output);
        //return new(output);
    }
    private static Hash256 ComputeHash(Stream stream, byte[] ioBuffer) {
        if (DebugMode) {
            return ComputeHashDebug(stream, ioBuffer);
        }
        else {
            return ComputeHashFast(stream, ioBuffer);
        }
    }
    private static Hash256 ComputeHashFast(ReadOnlySpan<byte> data) {
        unsafe {
            nint hasher = NativeBlakeWrapper.Create();
            try {
                _ = NativeBlakeWrapper.Init(hasher);
                fixed (byte* dataPtr = data) {
                    _ = NativeBlakeWrapper.Update(hasher, dataPtr, (nuint)data.Length);
                }
                Span<byte> output = stackalloc byte[32];
                fixed (byte* outputPtr = output) {
                    _ = NativeBlakeWrapper.Finalize(hasher, outputPtr, 32);
                }
                return new(output);
            }
            finally {
                NativeBlakeWrapper.Dispose(hasher);
            }
        }
    }
    private static Hash256 ComputeHashFast(Stream stream, byte[] ioBuffer) {
        unsafe {
            nint hasher = NativeBlakeWrapper.Create();
            try {
                int read = 0;
                fixed(byte * ioPtr = ioBuffer) {
                    _ = NativeBlakeWrapper.Init(hasher);
                    while ((read = stream.Read(ioBuffer, 0, ioBuffer.Length)) > 0) {
                        _ = NativeBlakeWrapper.Update(hasher, ioPtr, (nuint)read);
                    }
                    Span<byte> output = stackalloc byte[32];
                    fixed (byte* outputPtr = output) {
                        _ = NativeBlakeWrapper.Finalize(hasher, outputPtr, 32);
                    }
                    return new(output);
                }
            }
            finally {
                NativeBlakeWrapper.Dispose(hasher);
            }
        }
    }
    private static Hash256 ComputeHashDebug(Stream stream, byte[] ioBuffer) {
        unsafe {
            nint hasher = NativeBlakeWrapper.Create();
            int result = 0;
            long started = 0;
            try {
                fixed (byte* ioPtr = ioBuffer) {
                    result = NativeBlakeWrapper.Init(hasher);
                    while (true) {
                        started = Stopwatch.GetTimestamp();

                        int read = stream.Read(ioBuffer, 0, ioBuffer.Length);
                        Interlocked.Add(ref ReadTicks, Stopwatch.GetTimestamp() - started);
                        if (read == 0) {
                            break;
                        }

                        started = Stopwatch.GetTimestamp();

                        result = NativeBlakeWrapper.Update(hasher, ioPtr, (nuint)read);
                        Interlocked.Add(ref HashTicks, Stopwatch.GetTimestamp() - started);
                    }
                    started = Stopwatch.GetTimestamp();
                    Span<byte> output = stackalloc byte[32];
                    fixed (byte* outputPtr = output) {
                        result = NativeBlakeWrapper.Finalize(hasher, outputPtr, 32);
                    }
                    Interlocked.Add(ref FinalizeTicks, Stopwatch.GetTimestamp() - started);
                    return new(output);
                }
            }
            finally {
                NativeBlakeWrapper.Dispose(hasher);
            }
        }
    }
}
#else
using Blake3;
using MusicEco.Core.Types;

namespace MusicEco.Data.Services;

internal partial class Scanner {
    private static Hash256 ComputeHash(Memory<byte> data) {
        Span<byte> output = stackalloc byte[32];
        Hasher.Hash(data.Span, output);
        return new(output);
    }
    private static Hash256 ComputeHash(Stream stream, byte[] ioBuffer) {
        using (var harsher = Hasher.New()) {
            int read = 0;
            while ((read = stream.Read(ioBuffer, 0, ioBuffer.Length)) > 0) {
                harsher.Update(ioBuffer.AsSpan()[..read]);
            }
            Span<byte> output = stackalloc byte[32];
            harsher.Finalize(output);
            return new(output);
        }
    }
}
#endif