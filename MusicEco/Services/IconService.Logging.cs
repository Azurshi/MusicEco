using System.Diagnostics;

namespace MusicEco.Services;

public partial class IconService {
    private class CacheLog {
        public long HitCount { get; private set; }
        public long MissCount { get; private set; }
        public long TotalCount => HitCount + MissCount;
        public CacheLog() {
            this.HitCount = 0;
            this.MissCount = 0;
        }
        public void Miss() {
            this.MissCount++;
        }
        public void Hit() {
            this.HitCount++;
        }
        public void Log() {
            Debug.WriteLine($"Cache hit rate {100.0 * this.HitCount / this.TotalCount:F2}");
        }
        public void PeriodLog() {
            if (this.TotalCount % 10 == 0) {
                Debug.WriteLine($"Cache hit rate {100.0 * this.HitCount / this.TotalCount:F2}");
            }
        }
    }
    private class LoadLog {
        public long CompleteCount { get; private set; }
        public long CancelCount { get; private set; }
        public long TotalCount => CompleteCount + CancelCount;
        public LoadLog() {
            this.CompleteCount = 0;
            this.CancelCount = 0;
        }
        public void Complete() {
            this.CompleteCount++;
        }
        public void Cancel() {
            this.CancelCount++;
        }
        public void Log() {
            Debug.WriteLine($"Load complete rate {100.0 * this.CompleteCount / this.TotalCount:F2}");
        }
        public void PeriodLog() {
            if (TotalCount % 10 == 0) {
                Debug.WriteLine($"Load complete rate {100.0 * this.CompleteCount / this.TotalCount:F2}");
            }
        }
    }
}
