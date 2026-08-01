using System.Diagnostics;

namespace MusicEco.Services;

public partial class IconService {
    private class CacheLog {
        public long HitCount { get; private set; }
        public long MissCount { get; private set; }
        public long TotalCount => HitCount + MissCount;
        public CacheLog() {
            HitCount = 0;
            MissCount = 0;
        }
        public void Miss() {
            MissCount++;
        }
        public void Hit() {
            HitCount++;
        }
        public void Log() {
            Debug.WriteLine($"Cache hit rate {100.0 * HitCount / TotalCount:F2}");
        }
        public void PeriodLog() {
            if (TotalCount % 10 == 0) {
                Debug.WriteLine($"Cache hit rate {100.0 * HitCount / TotalCount:F2}");
            }
        }
    }
    private class LoadLog {
        public long CompleteCount { get; private set; }
        public long CancelCount { get; private set; }
        public long TotalCount => CompleteCount + CancelCount;
        public LoadLog() {
            CompleteCount = 0;
            CancelCount = 0;
        }
        public void Complete() {
            CompleteCount++;
        }
        public void Cancel() {
            CancelCount++;
        }
        public void Log() {
            Debug.WriteLine($"Load complete rate {100.0 * CompleteCount / TotalCount:F2}");
        }
        public void PeriodLog() {
            if (TotalCount % 10 == 0) {
                Debug.WriteLine($"Load complete rate {100.0 * CompleteCount / TotalCount:F2}");
            }
        }
    }
}
