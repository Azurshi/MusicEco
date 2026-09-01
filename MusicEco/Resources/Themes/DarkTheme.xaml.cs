namespace MusicEco.Resources.Themes;

using Localization = MusicEco.Services.Localization;

public partial class DarkTheme: ResourceDictionary, ITheme {
    public DarkTheme() {
        InitializeComponent();
    }
    public string Id => "dark";
    public string Name => Localization.L["Theme_Dark"];
    public ResourceDictionary GetResources() {
        return this;
    }
}