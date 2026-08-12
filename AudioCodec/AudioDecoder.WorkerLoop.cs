using System.Diagnostics;

namespace AudioCodec;

public partial class AudioDecoder {
    private void WorkerLoop() {
        while (Volatile.Read(ref this._disposed) <= 0) {
            // This code only invoked at entry, then lock into DecodeToPCM
            // Or run again when new stream is requested
            if (_streamControl.TryGetJob(out var stream)) {
                if (this._cts != null) {
                    this._cts.Cancel();
                    this._cts.Dispose();
                }
                this._cts = new();
                this.ResetSynchronize();    
                Debug.WriteLine("Schedule new job");
                var token = _cts.Token;
                this.DecodeToPCM(stream!, token); // This is what thread is running on
            }
            else {
                // This does not reach since thread is locked at DecodeToPCM
                // Only use when entry, to wait for start job
                try {
                    int index = WaitHandle.WaitAny([
                        this._streamControl.HaveJobEvent, 
                        this._disposeEvent
                        ]);
                    if (index == 0) {
                        // Continue
                        continue;
                    }
                    else if (index == 1) {
                        // Dispose
                        return;
                    }
                    else {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                catch (ThreadInterruptedException) {
                    Debug.WriteLine("Force exit");
                    return;
                }
            }
        }
    }
}
