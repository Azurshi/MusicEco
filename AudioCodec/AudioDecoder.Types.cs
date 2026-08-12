using AudioCodec.Enum;

namespace AudioCodec;

public sealed class DecoderConfig {
    public readonly int OutputChannels;
    public readonly int OutputSampleRate;
    public readonly AVSampleFormat OutputFormat;
    public readonly int IOBufferSize;
    public readonly int BytesPerSample;
    public readonly int BitsPerSample;
    public int BytesPerSecond => this.OutputSampleRate * this.OutputChannels * this.BytesPerSample;
    public int GetBytes(int sampleCount) {
        return sampleCount * OutputChannels * BytesPerSample;
    }
    public DecoderConfig(int outputChannels, int outputSampleRate, AVSampleFormat outputFormat, int ioBufferSize) {
        this.OutputChannels = outputChannels;
        this.OutputSampleRate = outputSampleRate;
        this.OutputFormat = outputFormat;
        this.IOBufferSize = ioBufferSize;
        this.BytesPerSample = FFmpegUtility.GetBytesPerSample(this.OutputFormat);
        this.BitsPerSample = 8 * this.BytesPerSample;
    }
}