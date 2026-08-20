using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class ThemeViewModel: ViewOnlyListItem, IUpdateble {
    public IComparable Identify => (this.ThemeId, this.ThemeName);

    public string ThemeId { get; set; }
    public string ThemeName { get; set; }
    public ThemeViewModel(string id, string name) {
        this.ThemeId = id;
        this.ThemeName = name;
    }
}