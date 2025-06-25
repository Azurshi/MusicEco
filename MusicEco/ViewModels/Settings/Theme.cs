using Domain.EventSystem;
using Domain.Models;
using MusicEco.Settings;
using System.Diagnostics;

namespace MusicEco.ViewModels.Settings;
public class ThemeSetting : BaseSetting {
    protected string previousTheme = "MAUI";
    public override string FieldName => nameof(ThemeSetting);
    public ThemeSetting(): base() { 
        EventSystem.Connect<AppSettingModel.SettingChangedEventArgs>(this.OnSettingChanged);
    }
    public override ISettingField Create() {
        ISettingField field = Create("string");
        field.UniqueName = GetUniqueName();
        field.Format = SettingFieldFormat.ComboBox;
        field.ValueDomain = ["MAUI", "Light", "Dark"];
        field.DefaultValue = "MAUI";
        field.Name = "Theme";
        field.Info = "Theme name";
        return field;
    }
    public void OnSettingChanged(object? sender, AppSettingModel.SettingChangedEventArgs args) {
        //if (SettingField == null) return;
        //string theme = AppSettingModel.Current.Theme;
        //if (previousTheme == theme) return;
        //Debug.WriteLine($"Change theme {theme}");
        //ResourceDictionary pallet;
        //if (theme == "Dark") {
        //    pallet = new DarkPallete();
        //} else if (theme == "Light") {
        //    pallet = new LightPallete();
        //} else {
        //    pallet = new DefaultPallete();
        //}
        //foreach (string key in pallet.Keys) {
        //    App.Current!.Resources[key] = pallet[key];
        //    Debug.WriteLine(key);
        //}
        //previousTheme = theme;
    }
}
