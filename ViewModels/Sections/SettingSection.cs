using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Common.Value;
using MusicEco.Global;
using MusicEco.Models;
using MusicEco.Models.Base;
using MusicEco.ViewModels.Base;
using MusicEco.ViewModels.Slots;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels.Sections;
public class SettingSection : PropertyObject {
    public SettingSection() {
        EventSystem.Connect<IntEventArgs>(Signal.UI_RowHeight_Changed, OnRowHeightChanged);
    }
    public int SlotHeight => Common.Value.UI.RowHeight;
    private void OnRowHeightChanged(object? sender, IntEventArgs e) {
        OnPropertyChanged(nameof(SlotHeight));
    }
}