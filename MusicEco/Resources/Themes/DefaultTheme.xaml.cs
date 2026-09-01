namespace MusicEco.Resources.Themes;

using Localization = MusicEco.Services.Localization;

public partial class DefaultTheme: ResourceDictionary, ITheme {
    public DefaultTheme() {
        InitializeComponent();
    }
    public string Id => "default";
    public string Name => Localization.L["Theme_Default"];
    public ResourceDictionary GetResources() {
        return this;
    }
}