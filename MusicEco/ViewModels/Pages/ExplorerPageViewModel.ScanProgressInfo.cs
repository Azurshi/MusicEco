using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Pages;

public partial class ExplorerPageViewModel {
    public sealed partial class ScanProgressInfo: ObservableObject {
        private int _scanFileCurrent = 0;
        public int ScanFileCurrent {
            get => this._scanFileCurrent;
            set {
                value = Math.Max(this._scanFileCurrent, value);
                if (this._scanFileCurrent != value) {
                    this._scanFileCurrent = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ScanFileProgress));
                    OnPropertyChanged(nameof(ScanFileProgressText));
                }
            }
        }
        private int _scanFileTotal = 0;
        public int ScanFileTotal {
            get => this._scanFileTotal;
            set {
                if (this._scanFileTotal != value) {
                    this._scanFileTotal = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ScanFileProgress));
                    OnPropertyChanged(nameof(ScanFileProgressText));
                }
            }
        }
        public double ScanFileProgress {
            get {
                if (this._scanFileTotal == 0) {
                    return 0;
                } else {
                    return (double)this._scanFileCurrent / this._scanFileTotal;
                }
            }
        }
        public string ScanFileProgressText {
            get {
                if (this._scanFileTotal == 0) {
                    return string.Empty;
                } else {
                    return $"{this._scanFileCurrent} / {this._scanFileTotal}";
                }
            }
        }
        private int _processFileCurrent = 0;
        public int ProcessFileCurrent {
            get => this._processFileCurrent;
            set {
                value = Math.Max(this._processFileCurrent, value);
                if (this._processFileCurrent != value) {
                    this._processFileCurrent = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProcessFileProgress));
                    OnPropertyChanged(nameof(ProcessFileProgressText));
                }
            }
        }
        private int _processFileTotal = 0;
        public int ProcessFileTotal {
            get => this._processFileTotal;
            set {
                if (this._processFileTotal != value) {
                    this._processFileTotal = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProcessFileProgress));
                    OnPropertyChanged(nameof(ProcessFileProgressText));
                }
            }
        }
        public double ProcessFileProgress {
            get {
                if (this._processFileTotal == 0) {
                    return 0;
                } else {
                    return (double)this._processFileCurrent / this._processFileTotal;
                }
            }
        }
        public string ProcessFileProgressText {
            get {
                if (this._processFileTotal == 0) {
                    return string.Empty;
                } else {
                    return $"{this._processFileCurrent} / {this._processFileTotal}";
                }
            }
        }
        private int _saveDataCurrent = 0;
        public int SaveDataCurrent {
            get => this._saveDataCurrent;
            set {
                value = Math.Max(this._saveDataCurrent, value);
                if (this._saveDataCurrent != value) {
                    this._saveDataCurrent = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SaveDataProgress));
                    OnPropertyChanged(nameof(SaveDataProgressText));
                }
            }
        }
        private int _saveDataTotal = 0;
        public int SaveDataTotal {
            get => this._saveDataTotal;
            set {
                if (this._saveDataTotal != value) {
                    this._saveDataTotal = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SaveDataProgress));
                    OnPropertyChanged(nameof(SaveDataProgressText));
                }
            }
        }
        public double SaveDataProgress {
            get {
                if (this._saveDataTotal == 0) {
                    return 0;
                } else {
                    return (double)this._saveDataCurrent / this._saveDataTotal;
                }
            }
        }
        public string SaveDataProgressText {
            get {
                if (this._saveDataTotal == 0) {
                    return string.Empty;
                } else {
                    return $"{this._saveDataCurrent} / {this._saveDataTotal}";
                }
            }
        }
        public void Reset() {
            this._scanFileCurrent = 0;
            this._scanFileTotal = 0;
            this._processFileCurrent = 0;
            this._processFileTotal = 0;
            this._saveDataCurrent = 0;
            this._saveDataTotal = 0;
            string[] propertyNames = [
                nameof(ScanFileCurrent), nameof(ScanFileTotal), nameof(ScanFileProgress), nameof(ScanFileProgressText),
                nameof(ProcessFileCurrent), nameof(ProcessFileTotal), nameof(ProcessFileProgress), nameof(ProcessFileProgressText),
                nameof(SaveDataCurrent), nameof(SaveDataTotal), nameof(SaveDataProgress), nameof(SaveDataProgressText)
                ];
            foreach (var propertyName in propertyNames) {
                OnPropertyChanged(propertyName);
            }
        }
        public ScanProgressInfo() {

        }
    }
}
