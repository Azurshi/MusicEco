using Domain.Models;

namespace MusicEco.ViewModels.Settings;
public class GridPreloadSetting : BaseSetting {
    public override string FieldName => nameof(GridPreloadSetting);
    public override ISettingField Create() {
        ISettingField field = Create("int");
        field.UniqueName = GetUniqueName();
        field.Format = SettingFieldFormat.Slider;
        field.ValueDomain = [1, 25, 1];
        field.DefaultValue = 4;
        field.Name = "Grid rows preload";
        field.Info = "Number of rows in grid preloaded per page";
        return field;
    }
}
