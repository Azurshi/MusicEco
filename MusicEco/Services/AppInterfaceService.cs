using MusicEco.Core.Services;
using MusicEco.Views.Shell;

namespace MusicEco.Services;

internal class AppInterfaceService: IAppInterfaceService {
    private const DisplayOrientation DefaultOrientation
#if WINDOWS || MACCATALYST
        = DisplayOrientation.Landscape;
#elif ANDROID || IOS
        = DisplayOrientation.Portrait;
#endif
    private const float DefaultScale = 1.0f;
    private readonly IAppSetting _setting;
    public event EventHandler<float>? ScaleChanged;
    public event EventHandler<DisplayOrientation>? OrientationChanged;
    public AppInterfaceService(IAppSetting appSetting) {
        this._setting = appSetting;
    }
    public DisplayOrientation GetOrientation() {
        return this._setting.Get(DefaultOrientation, nameof(DisplayOrientation));
    }
    public void SetOrientation(DisplayOrientation orientation) {
        if (this.GetOrientation() != orientation) {
            this._setting.Set(orientation, nameof(DisplayOrientation));
            OrientationChanged?.Invoke(this, orientation);
        }
    }
    public void LoadLastOrientation() {
        var lastOrientation = this.GetOrientation();
        OrientationChanged?.Invoke(this, lastOrientation);
    }

    public float GetScale() {
        return this._setting.Get(DefaultScale, "InterfaceScale");
    }
    public void SetScale(float scale) {
        if (this.GetScale() != scale) {
            this._setting.Set(scale, "InterfaceScale");
            var app = (App?)App.Current;
            app?.SetScale(scale);
            ScaleChanged?.Invoke(this, scale);
        }
    }
    public void LoadLastScale() {
        var lastScale = this.GetScale();
        var app = (App?)App.Current;
        app?.SetScale(lastScale);
        ScaleChanged?.Invoke(this, lastScale);
    }
}
