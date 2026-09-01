using AudioCodec.Types;
using System.Diagnostics;

namespace AudioCodec;

public partial class AudioDecoder {
    /// <summary>
    /// Start synchronize process. Wait for consumer thread to synchronize.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal int ResetSynchronize() {
        this.ResetRequested.Set();
        Debug.WriteLine("Decoder: Reset synchronize request");
        int index = WaitHandle.WaitAny([
            this.ResetCompleted,
            this._disposeEvent
            ]);
        if (index == 0) {
            // Synchronize success
            // Seek completed
            Debug.WriteLine("Decoder: Reset synchronize completed");
            this.ResetRequested.Reset();
            this.ResetCompleted.Reset();
            return 0;
        }
        else if (index == 1) {
            // Receive dispose signal while synchronize
            return -1;
        }
        else {
            throw new InvalidOperationException();
        }
    }
    internal unsafe int DecodeLoop(
        AVFormatContext* format,
        AVCodecContext* codec,
        SwrContext* resampler,
        AVPacket* packet,
        AVFrame* frame,
        int audioStreamIndex,
        AVRational timeBase,
        CancellationToken token
        ) {
        if (Volatile.Read(ref this._disposed) > 0) {
            Debug.WriteLine("AudioDecoder disposed");
            return -1;
        }
        if (this._streamControl.HaveJob()) {
            // Return control to WorkerLoop
            return -1;
        }
        if (token.IsCancellationRequested) {
            Debug.WriteLine("Decode cancelled");
            return -1;
        }
        if (this._seekControl.TryTakeSeek(out TimeSpan target)) {
            this.SeekDecoder(
                format,
                codec,
                resampler,
                packet,
                frame,
                audioStreamIndex,
                timeBase,
                target);
            int res = this.ResetSynchronize();
            if (res < 0) {
                return -1;
            }
        }
        int result = FFmpeg.Format.av_read_frame(format, packet);
        if (result == FFmpeg.Flags.AVERR_EOF) {
            DecodeEnd?.Invoke(this, EventArgs.Empty);
            try {
                int index = WaitHandle.WaitAny([
                    this._streamControl.HaveJobEvent, 
                    this._seekControl.HaveJobEvent, 
                    this._disposeEvent
                    ]);
                if (index == 0) {
                    // Move to if(_streamControl.HaveJob())
                    return 0;
                }
                else if (index == 1) {
                    // Move to next cycle
                    return 0;
                }
                else if (index == 2) {
                    // Dispose
                    return -1;
                }
                else {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch (ThreadInterruptedException) {
                Debug.WriteLine("Force exit");
                return -1;
            }
        }
        else {
            if (packet->StreamIndex == audioStreamIndex) {
                result = FFmpeg.Codec.avcodec_send_packet(codec, packet);
                FFmpegUtility.CheckResult(result);
                this.ReceiveFrames(codec, resampler, frame, timeBase, false);
            }
            FFmpeg.Codec.av_packet_unref(packet);
            return 0;
        }
    }
}
