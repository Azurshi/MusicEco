using Domain.EventSystem;
using Domain.Models;

namespace MusicEco.ViewModels.Settings;
public class GridRowsSetting : BaseSetting {
    public override string FieldName => nameof(GridRowsSetting);
    public override ISettingField Create() {
        ISettingField field = Create("int");
        field.UniqueName = GetUniqueName();
        field.Format = SettingFieldFormat.Slider;
        field.ValueDomain = [1, 16, 1];
        field.DefaultValue = 4;
        field.Name = "Grid row amount";
        field.Info = "Number of rows in grid per page";
        return field;
    }
    public GridRowsSetting() {
        EventSystem.Connect<MusicEco.ViewModels.AppSettingModel.SettingChangedEventArgs>(
            (s, e) => {
                Refresh();
                UpdateResource();
            });
    }

    [StaticInitializer(0)]
    public static void Initialize() {
        EventSystem.Connect<AppShell.WindowSizeChangedEventArgs>(OnWindowSizeChanged);
    }
    private static void OnWindowSizeChanged(object? sender, AppShell.WindowSizeChangedEventArgs e) {
        UpdateResource();
    }
    private static void UpdateResource() {
        AppSettingModel settings = AppSettingModel.Current;
        ResourceDictionary resources = Application.Current!.Resources;
        resources["GridItemSize"] = AppShell.ScreenHeight / settings.GridRows;
    }
}
