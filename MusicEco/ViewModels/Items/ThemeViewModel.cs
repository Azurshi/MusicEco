using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

public partial class ThemeViewModel: ObservableObject, IUpdateble, ISelectableItem {
    public object Identify => this.ThemeId;

    public string ThemeId { get; set; }
    public string ThemeName { get; set; }
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
    public ThemeViewModel(string id, string name) {
        this.ThemeId = id;
        this.ThemeName = name;
    }
}