using Domain.EventSystem;
using Domain.Models;
using MusicEco.ViewModels.Pages;
using System.Diagnostics;

namespace MusicEco.ViewModels.Settings;
public class ListItemsSetting : BaseSetting, IServiceAccess {
    public override string FieldName => nameof(ListItemsSetting);
    public override ISettingField Create() {
        ISettingField field = Create("int");
        field.UniqueName = GetUniqueName();
        field.Format = SettingFieldFormat.Slider;
        field.ValueDomain = [4, 20, 1];
        field.DefaultValue = 12;
        field.Name = "List row amount";
        field.Info = "Number of rows in list per page";
        return field;
    }
    public ListItemsSetting() {
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
        resources["ListItemSize"] = AppShell.ScreenHeight / settings.ListItems;
    }
}
