using Domain.Models;

namespace MusicEco.ViewModels.Settings;
public class GridColumnsSetting : BaseSetting {
    public override string FieldName => nameof(GridColumnsSetting);
    public override ISettingField Create() {
        ISettingField field = Create("int");
        field.UniqueName = GetUniqueName();
        field.Format = SettingFieldFormat.Slider;
        field.ValueDomain = [1, 16, 1];
        field.DefaultValue = 4;
        field.Name = "Grid column amount";
        field.Info = "Number of column in grid";
        return field;
    }
}
