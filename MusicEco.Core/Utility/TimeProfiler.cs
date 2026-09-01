using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MusicEco.Core.Utility;

public class TimeProfiler: IDisposable {
    private readonly long _startTimeStamp;
    private readonly string _name;
    private readonly bool _enabled;
    private readonly string _unit;
    public TimeProfiler(bool enabled = true, string unit = "ms", [CallerMemberName] string name = "") {
        this._enabled = enabled;
        if (enabled) {
            if (unit == "ms" || unit == "s") {
                this._startTimeStamp = Stopwatch.GetTimestamp();
                this._name = name;
                this._unit = unit;
            }
            else {
                throw new ArgumentOutOfRangeException(nameof(unit));
            }
        }
        else {
            this._startTimeStamp = 0;
            this._name = string.Empty;
            this._unit = string.Empty;
        }
    }

    public void Dispose() {
        if (this._enabled) {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(this._startTimeStamp);
            if (this._unit == "ms") {
                Debug.WriteLine($"--- Time profile: {this._name} : {elapsed.TotalMilliseconds:F2} ms");
            }
            else if (this._unit == "s") {
                Debug.WriteLine($"--- Time profile: {this._name} : {elapsed.TotalSeconds:F2} s");
            }
        }
        GC.SuppressFinalize(this);
    }
}
