namespace MusicEco.Services;

internal partial class AppInterfaceService {
    public event EventHandler<OrientationItem>? OrientationChanged;
    private const string OrientationStorageField = "DisplayOrientation";
    private const DisplayOrientation DefaultOrientation
#if WINDOWS || MACCATALYST
        = DisplayOrientation.Landscape;
#elif ANDROID || IOS
    = DisplayOrientation.Portrait;
#endif
    private string GetOrientationName(DisplayOrientation orientation) {
        return orientation switch {
            DisplayOrientation.Landscape => this.L["Setting_Interface_Orientation_Landscape"],
            DisplayOrientation.Portrait => this.L["Setting_Interface_Orientation_Portrait"],
            _ => throw new ArgumentOutOfRangeException(nameof(orientation))
        };
    }
    public OrientationItem GetOrientation() {
        var value = this._setting.Get(DefaultOrientation, OrientationStorageField);
        return new(value, GetOrientationName(value));
    }
    public IReadOnlyList<OrientationItem> GetOrientations() {
        return [
            new(DisplayOrientation.Landscape, GetOrientationName(DisplayOrientation.Landscape)),
            new(DisplayOrientation.Portrait, GetOrientationName(DisplayOrientation.Portrait))
            ];
    }
    public void SetOrientation(DisplayOrientation orientation) {
        if (this.GetOrientation().Orientation != orientation) {
            var name = this.GetOrientationName(orientation);
            this._setting.Set(orientation, OrientationStorageField);
            OrientationChanged?.Invoke(this, new(orientation, name));
        }
    }
    public void LoadLastOrientation() {
        var lastOrientation = this.GetOrientation();
        OrientationChanged?.Invoke(this, lastOrientation);
    }
}
