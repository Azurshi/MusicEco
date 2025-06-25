using Domain.Models;

namespace MusicEco.ViewModels.Settings;
public class ListPreloadSetting : BaseSetting {
    public override string FieldName => nameof(ListPreloadSetting);
    public override ISettingField Create() {
        ISettingField field = Create("int");
        field.UniqueName = GetUniqueName();
        field.Format = SettingFieldFormat.Slider;
        field.ValueDomain = [1, 50, 1];
        field.DefaultValue = 20;
        field.Name = "List preload amount";
        field.Info = "Number of item in list preloaded per page";
        return field;
    }
}
