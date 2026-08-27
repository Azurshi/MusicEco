using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class LanguageViewModel: ViewOnlyListItem, IUpdateble {
    public IComparable Identify => this.LanguageCode;

    public string LanguageCode { get; init; }
    public string LanguageName { get; init; }
    public LanguageViewModel(string code, string name) {
        this.LanguageCode = code;
        this.LanguageName = name;
    }
}
