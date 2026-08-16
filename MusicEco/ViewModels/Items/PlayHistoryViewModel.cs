using MusicEco.Core.Types;
using MusicEco.Services;

namespace MusicEco.ViewModels.Items;

public partial class PlayHistoryViewModel: AudioEntryViewModel {
    private sealed class PlayHistoryIdentify(Hash256 fileHash, DateTime time): IComparable {
        public readonly Hash256 FileHash = fileHash;
        public readonly DateTime Time = time;
        public int CompareTo(object? obj) {
            if (obj is PlayHistoryIdentify other) {
                int result = FileHash.CompareTo(other.FileHash);
                if (result == 0) {
                    result = Time.CompareTo(other.Time);
                }
                return result;
            }
            else {
                return -1;
            }
        }
    }
    public override object Identify => (FileHash, LastPlayedTime);
    private readonly TimeFormatter _formatter;
    public DateTime LastPlayedTime { get; init; }
    public string LastPlayedText => this._formatter.Different(LastPlayedTime);
    public PlayHistoryViewModel(Hash256 fileHash, string displayTitle, DateTime lastPlayedTime) : base(fileHash, displayTitle) {
        this._formatter = AppLifeCycle.Provider.GetRequiredService<TimeFormatter>();
        this.LastPlayedTime = lastPlayedTime;
    }
    public void RefreshNotify() {
        OnPropertyChanged(nameof(LastPlayedText));
    }
}
