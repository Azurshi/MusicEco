using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class LanguageViewModel: ViewOnlyListItem, IUpdateble {
    public IComparable Identify => this.LanguageCode;

    public string LanguageCode { get; set; }
    public string LanguageName { get; set; }
    public LanguageViewModel(string code, string name) {
        this.LanguageCode = code;
        this.LanguageName = name;
    }
}
