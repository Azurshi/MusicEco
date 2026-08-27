namespace MusicEco.ViewModels.Pages.Settings;


public abstract class BaseInterfaceItemViewModel {
    public string Text { get; init; }
    public BaseInterfaceItemViewModel(string text) {
        this.Text = text;
    }
    public override string ToString() {
        return this.Text;
    }
}

public class ThemeItemViewModel: BaseInterfaceItemViewModel {
    public string ThemeId { get; init; }
    public ThemeItemViewModel(string id, string text) : base(text) {
        this.ThemeId = id;
    }
}

public class ScaleItemViewModel: BaseInterfaceItemViewModel {
    public float Value { get; init; }
    public ScaleItemViewModel(float value, string text): base(text) {
        this.Value = value;
    }
}
public class OrientationItemViewModel: BaseInterfaceItemViewModel {
    public DisplayOrientation Value { get; init; }
    public OrientationItemViewModel(DisplayOrientation orientation, string text) : base(text) {
        this.Value = orientation;
    }
}