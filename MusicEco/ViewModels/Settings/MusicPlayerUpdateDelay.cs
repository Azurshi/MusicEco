using Domain.Models;

namespace MusicEco.ViewModels.Settings;
public class MusicPlayerUpdateDelaySetting : BaseSetting {
    public override string FieldName => nameof(MusicPlayerUpdateDelaySetting);
    public override ISettingField Create() {
        ISettingField field = Create("int");
        field.UniqueName = GetUniqueName();
        field.Format = SettingFieldFormat.ComboBox;
        field.ValueDomain = [10, 50, 100, 200, 300, 400, 500, 1000, 10000];
        field.DefaultValue = 1000;
        field.Name = "Music player progress update delay";
        field.Info = "Music player progress update delay in ms";
        return field;
    }
}
