namespace MusicEco.Services;

internal partial class AppInterfaceService {
    private const float DefaultScale = 1.0f;
    private const string ScaleStorageField = "InterfaceScale";
    public event EventHandler<ScaleItem>? ScaleChanged;
    private string GetScaleText(float scale) {
        string format = this.L["Setting_Interface_Scale_Template"];
        return string.Format(format, (int)(scale * 100));
    }
    public ScaleItem GetScale() {
        var value = this._setting.Get(DefaultScale, ScaleStorageField);
        return new(value, this.GetScaleText(value));
    }
    public IReadOnlyList<ScaleItem> GetScales() {
        List<ScaleItem> items = [];
        foreach(var supportedScale in Config.SupportedScales) {
            items.Add(new(supportedScale, this.GetScaleText(supportedScale)));
        }
        return items;
    }
    public void SetScale(float scale) {
        if (this.GetScale().Value != scale) {
            this.App.SetScale(scale);
            var text = this.GetScaleText(scale);
            this._setting.Set(scale, ScaleStorageField);
            ScaleChanged?.Invoke(this, new(scale, text));
        }
    }
    public void LoadLastScale() {
        var lastScale = this.GetScale();
        this.App.SetScale(lastScale.Value);
        ScaleChanged?.Invoke(this, lastScale);
    }
}
