namespace MusicEco.Resources.Themes;

using Localization = MusicEco.Services.Localization;

public partial class LightTheme: ResourceDictionary, ITheme {
    public LightTheme() {
        InitializeComponent();
    }
    public string Id => "light";
    public string Name => Localization.L["Theme_Light"];
    public ResourceDictionary GetResources() {
        return this;
    }
}