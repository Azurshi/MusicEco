using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class LanguageViewModel: ObservableObject, IUpdateble, ISelectableItem {
    public object Identify => this.LanguageCode;

    public string LanguageCode { get; set; }
    public string LanguageName { get; set; }
    private bool _selected = false;
    public bool Selected {
        get => this._selected;
        set {
            if (this._selected != value) {
                this._selected = value;
                OnPropertyChanged();
            }
        }
    }

    public LanguageViewModel(string code, string name) {
        this.LanguageCode = code;
        this.LanguageName = name;
    }
}
