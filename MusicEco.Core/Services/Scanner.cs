namespace MusicEco.Core.Services;

public readonly struct ScanFileProgress {
    public readonly int Current;
    public readonly int Total;
    public ScanFileProgress(int current, int total) {
        this.Current = current;
        this.Total = total;
    }
}
public record ProcessFileProgress {
    public readonly int Current;
    public readonly int Total;
    public ProcessFileProgress(int current, int total) {
        this.Current = current;
        this.Total = total;
    }
}
public readonly struct PushChangeProgress {
    public readonly int Current;
    public readonly int Total;
    public PushChangeProgress(int current, int total) {
        this.Current = current;
        this.Total = total;
    }
}
public class ScanProgress {
    public readonly IProgress<ScanFileProgress> ScanFile;
    public readonly IProgress<ProcessFileProgress> ProcessFile;
    public readonly IProgress<PushChangeProgress> PushChange;
    public ScanProgress(IProgress<ScanFileProgress> scanFile, IProgress<ProcessFileProgress> processFile, IProgress<PushChangeProgress> pushChange) {
        this.ScanFile = scanFile;
        this.ProcessFile = processFile;
        this.PushChange = pushChange;
    }
    public ScanProgress(Action<ScanFileProgress> scanFile, Action<ProcessFileProgress> processFile, Action<PushChangeProgress> pushChange) {
        this.ScanFile = new Progress<ScanFileProgress>(scanFile);
        this.ProcessFile = new Progress<ProcessFileProgress>(processFile);
        this.PushChange = new Progress<PushChangeProgress>(pushChange);
    }
}
public interface IScanResult {
    public bool Success { get; }
}
public interface IScanner {
    public event EventHandler<bool>? RunningChanged;
    // Block delete AudioEntity and modify FileEntity while running
    public bool Running { get; }
    // Scan, compare with exising data and update
    public Task<IScanResult> ScanAndUpdate(ScanProgress progress, List<string> fileExtensions, int scanWorkers, int processWorkers, TimeSpan updateInterval, object? caller = null, bool verbose = false);
}
public interface IScanPathService {
    public event EventHandler ItemChanged;
    public Task<IReadOnlyList<string>> GetPaths();
    public Task<bool> AddPath(string path);
    public Task<bool> RemovePath(string path);
}
