using System.Runtime.InteropServices;

namespace AudioCodec.Types;


[StructLayout(LayoutKind.Sequential)]
public unsafe struct ByteArray4 {
    public const int Size = 4;
    public readonly int Length => Size;
    fixed byte Data[Size];
    public byte this[uint i] {
        get => Data[i];
        set => Data[i] = value;
    }
    public byte[] ToArray() {
        var result = new byte[Size];
        for (int i = 0; i < Size; i++) {
            result[i] = Data[i];
        }
        return result;
    }
    public void UpdateFrom(byte[] array) {
        for (int i = 0; i < Size; i++) {
            Data[i] = array[i];
        }
    }
    public static implicit operator byte[](ByteArray4 arr) => arr.ToArray();
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ByteArray8 {
    public const int Size = 8;
    public readonly int Length => Size;
    fixed byte Data[Size];
    public byte this[uint i] {
        get => Data[i];
        set => Data[i] = value;
    }
    public byte[] ToArray() {
        var result = new byte[Size];
        for (int i = 0; i < Size; i++) {
            result[i] = Data[i];
        }
        return result;
    }
    public void UpdateFrom(byte[] array) {
        for (int i = 0; i < Size; i++) {
            Data[i] = array[i];
        }
    }
    public static implicit operator byte[](ByteArray8 arr) => arr.ToArray();
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ByteArray16 {
    public const int Size = 16;
    public readonly int Length => Size;
    fixed byte Data[Size];
    public byte this[uint i] {
        get => Data[i];
        set => Data[i] = value;
    }
    public byte[] ToArray() {
        var result = new byte[Size];
        for(int i=0;i<Size;i++) {
            result[i] = Data[i];
        }
        return result;
    }
    public void UpdateFrom(byte[] array) {
        for (int i = 0; i < Size; i++) {
            Data[i] = array[i];
        }
    }
    public static implicit operator byte[](ByteArray16 arr) => arr.ToArray();
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct IntArray8 {
    public const int Size = 8;
    public readonly int Length => Size;
    fixed int Data[Size];
    public int this[uint i] {
        get => Data[i];
        set => Data[i] = value;
    }
    public int[] ToArray() {
        var result = new int[Size];
        for (int i = 0; i < Size; i++) {
            result[i] = Data[i];
        }
        return result;
    }
    public void UpdateFrom(int[] array) {
        for (int i = 0; i < Size; i++) {
            Data[i] = array[i];
        }
    }
    public static implicit operator int[](IntArray8 arr) => arr.ToArray();
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct ULongArray8 {
    public const int Size = 8;
    public readonly int Length => Size;
    fixed ulong Data[Size];
    public ulong this[uint i] {
        get => Data[i];
        set => Data[i] = value;
    }
    public ulong[] ToArray() {
        var result = new ulong[Size];
        for (int i = 0; i < Size; i++) {
            result[i] = Data[i];
        }
        return result;
    }
    public void UpdateFrom(ulong[] array) {
        for (int i = 0; i < Size; i++) {
            Data[i] = array[i];
        }
    }
    public static implicit operator ulong[](ULongArray8 arr) => arr.ToArray();
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct BytePointerArray8 {
    public const int Size = 8;
    public readonly int Length => Size;
    byte* _0; byte* _1; byte* _2; byte* _3; byte* _4; byte* _5; byte* _6; byte* _7;
    public byte* this[uint i] {
        get {
            fixed (byte** p0 = &_0) {
                return *(p0 + i);
            }
        }
        set {
            fixed (byte** p0 = &_0) {
                *(p0 + i) = value;
            }
        }
    }
    public byte*[] ToArray() {
        var result = new byte*[Size];
        for (int i = 0; i < Size; i++) {
            result[i] = this[(uint)i];
        }
        return result;
    }
    public void UpdateFrom(byte*[] array) {
        for (int i = 0; i < Size; i++) {
            this[(uint)i] = array[i];
        }
    }
    public static implicit operator byte*[](BytePointerArray8 arr) => arr.ToArray();
}


[StructLayout(LayoutKind.Sequential)]
public unsafe struct AVBufferRefPtrArray8 {
    public const int Size = 8;
    public readonly int Length => Size;
    AVBufferRef* _0; AVBufferRef* _1; AVBufferRef* _2; AVBufferRef* _3; AVBufferRef* _4; AVBufferRef* _5; AVBufferRef* _6; AVBufferRef* _7;
    public AVBufferRef* this[uint i] {
        get {
            fixed(AVBufferRef** p0 = &_0) {
                return *(p0 + i);
            }
        }
        set {
            fixed(AVBufferRef** p0 = &_0) {
                *(p0 + i) = value;
            }
        }
    }
    public AVBufferRef*[] ToArray() {
        var result = new AVBufferRef*[Size];
        for (int i = 0; i < Size; i++) {
            result[i] = this[(uint)i];
        }
        return result;
    }
    public void UpdateFrom(AVBufferRef*[] array) {
        for (int i = 0; i < Size; i++) {
            this[(uint)i] = array[i];
        }
    }
    public static implicit operator AVBufferRef*[](AVBufferRefPtrArray8 arr) => arr.ToArray();
}