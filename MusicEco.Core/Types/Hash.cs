using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MusicEco.Core.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Hash256: IEquatable<Hash256> {
    private readonly ulong A, B, C, D;
    public static bool operator ==(Hash256 left, Hash256 right) {
        return left.A == right.A && left.B == right.B && left.C == right.C && left.D == right.D;
    }
    public static bool operator !=(Hash256 left, Hash256 right) {
        return left.A != right.A || left.B != right.B || left.C != right.C || left.D != right.D;
    }
    public override readonly int GetHashCode() {
        return HashCode.Combine(A, B, C, D);
    }
    ///// <summary>
    ///// The span returned must have equal or less lifespan than <see cref="Hash256"/> struct.
    ///// </summary>
    ///// <returns></returns>
    //[UnscopedRef]
    //public Span<byte> AsSpan() {
    //    return MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref this, 1));
    //}
    public Hash256(ReadOnlySpan<byte> source) {
        if (source.Length != 32) {
            throw new ArgumentException("Source must be 32 bytes");
        }
        this = MemoryMarshal.Read<Hash256>(source);
    }
    public readonly ReadOnlySpan<byte> AsReadOnlySpan() {
        return MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(in this, 1));
    }
    public readonly override bool Equals(object? obj) {
        if (obj is Hash256 hash) {
            return A == hash.A && B == hash.B && C == hash.C && D == hash.D;
        }
        else {
            return false;
        }
    }

    public bool Equals(Hash256 other) {
        return A == other.A && B == other.B && C == other.C && D == other.D;
    }

    //public int Compare(Hash256 x, Hash256 y) {
    //    int result = x.A.CompareTo(y.A);
    //    if (result != 0) {
    //        result = x.B.CompareTo(y.B);
    //        if (result != 0) {
    //            result = x.C.CompareTo(y.C);
    //            if (result != 0) {
    //                result = x.D.CompareTo(y.D);
    //            }
    //        }
    //    }
    //    return result;
    //}
}