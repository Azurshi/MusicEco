using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.SourceGeneration;
using System.Diagnostics;
using System.Text;

namespace MusicEco.Services;

internal partial class PlaybackTrackingService: IPlaybackTrackingService {
    private readonly IAppSetting _setting;
    private readonly IPlayEventService _eventService;
    [AppSettingProperty(null, IsObservableObject = false)]
    private partial Hash256? RecordHash { get; set; }
    [AppSettingProperty(null, IsObservableObject = false)]
    private partial DateTime? RecordTime { get; set; }
    public PlaybackTrackingService(IAppSetting appSetting, IPlayEventService playEventService) {
        this._setting = appSetting;
        this._eventService = playEventService;
    }
    public async Task Record(TimeSpan position, TimeSpan duration) {
        var hash = this.RecordHash;
        var time = this.RecordTime;
        if (hash != null && time != null) {
            this.RecordHash = null;
            this.RecordTime = null;
            PlayEvent playEvent = new(time.Value, hash.Value, position, (float)position.Ticks / duration.Ticks);
            var success = await this._eventService.Insert(playEvent, this);
            if (success) {
                string format = @"hh\:mm\:ss";
                Debug.WriteLine($"Tracking: Record {position.ToString(format)} / {duration.ToString(format)} | {Encoding.UTF8.GetString(hash.Value.AsReadOnlySpan())}");
            } else {
                Debug.WriteLine($"Tracking: Record failed");
            }
        }
    }

    public void Start(Hash256 fileHash) {
        this.RecordHash = fileHash;
        this.RecordTime = DateTime.UtcNow;
    }
}
