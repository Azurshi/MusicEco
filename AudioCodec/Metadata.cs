namespace AudioCodec;

public readonly struct AudioMetadata(TimeSpan duration) {
    public readonly TimeSpan Duration = duration;
}
public readonly ref struct PacketData(ReadOnlySpan<byte> data, long pts) {
    public readonly ReadOnlySpan<byte> Data = data;
    public readonly long TicksPTS = pts;
}