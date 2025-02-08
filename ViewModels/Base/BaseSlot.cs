using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global;

namespace MusicEco.ViewModels.Base;
public abstract class BaseSlot : PropertyObject {
    public BaseSlot() {
        EventSystem.Connect<IntEventArgs>(Signal.UI_RowHeight_Changed, OnRowHeightChanged);
    }
    public int SlotHeight => Common.Value.UI.RowHeight;
    private void OnRowHeightChanged(object? sender, IntEventArgs e) {
        OnPropertyChanged(nameof(SlotHeight));
    }
    protected string? _key;
    public string Key {
        get => _key ?? string.Empty;
        set {
            if (_key != value) {
                _key = value;
                if (IsActive) OnActive();
                OnPropertyChanged();
            }
        }
    }
    protected virtual async Task OnActive() {
        await Task.Delay(999);
        throw new NotImplementedException();
    }

    protected bool isActive = false;
    public bool IsActive => isActive;
    public virtual async Task Active() {
        isActive = true;
        await OnActive();
    }
}