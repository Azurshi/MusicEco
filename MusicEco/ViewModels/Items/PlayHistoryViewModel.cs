using MusicEco.Core.Types;
using MusicEco.Services;

namespace MusicEco.ViewModels.Items;

public partial class PlayHistoryViewModel: AudioEntryViewModel {
    public override IComparable Identify => (FileHash, LastPlayedTime);
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
