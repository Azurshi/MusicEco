using System.Buffers;
using System.Buffers.Text;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MusicEco.Core.Types;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Hash256: IEquatable<Hash256>, IComparable {
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

    public int CompareTo(object? obj) {
        if (obj is Hash256 other) {
            int result = A.CompareTo(other.A);
            if (result == 0) {
                result = B.CompareTo(other.B);
                if (result == 0) {
                    result = C.CompareTo(other.C);
                    if (result == 0) {
                        result = D.CompareTo(other.D);
                    }
                }
            }
            return result;
        }
        else {
            return -1;
        }
    }
}

public class Hash256JsonConverter: JsonConverter<Hash256> {
    public override Hash256 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        if (reader.TokenType == JsonTokenType.Null) {
            return default;
        }
        Span<byte> buffer = stackalloc byte[44];
        int encodedLength = reader.CopyString(buffer);
        if (encodedLength != 44) {
            throw new JsonException("Invalid encoded length");
        }
        OperationStatus status = Base64.DecodeFromUtf8InPlace(buffer, out int bytesWritten);
        if (status != OperationStatus.Done || bytesWritten != 32) {
            throw new JsonException("Invalid Base64 data");
        }
        return new Hash256(buffer[..32]);
    }

    public override void Write(Utf8JsonWriter writer, Hash256 value, JsonSerializerOptions options) {
        writer.WriteBase64StringValue(value.AsReadOnlySpan());
    }
}