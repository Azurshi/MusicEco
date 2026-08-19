using MusicEco.Core.Services;

namespace MusicEco.Data;

public class SimpleScanResult: IScanResult {
    public bool Success { get; init; }
    internal SimpleScanResult(bool success) {
        this.Success = success;
    }
}

public class DetailScanResult: IScanResult {
    public bool Success { get; init; }
    public TimeSpan ScanFileTime { get; init; }
    public TimeSpan ProcessFileTime { get; init; }
    public TimeSpan SaveDataTime { get; init; }

    public object? Extra { get; init; } = null;
    internal DetailScanResult(bool success) {
        this.Success = success;
    }
    internal DetailScanResult(TimeSpan scanFileTime, TimeSpan processFileTime, TimeSpan saveData) {
        this.Success = true;
        this.ScanFileTime = scanFileTime;
        this.ProcessFileTime = processFileTime;
        this.SaveDataTime = saveData;
    }
    internal DetailScanResult(TimeSpan scanFileTime, TimeSpan processFileTime, TimeSpan saveData, object extra): this(scanFileTime, processFileTime, saveData) {
        this.Extra = extra;
    }
}