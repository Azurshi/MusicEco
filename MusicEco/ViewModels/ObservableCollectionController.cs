using MusicEco.ViewModels.Items;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels;
public class ObservableCollectionController<TItem>(ObservableCollection<BaseItem> target) where TItem : BaseItem, new() {
    public event EventHandler? KeyUpdated;
    public readonly ObservableCollection<BaseItem> Target = target;
    private bool _isPagingBusy = false;
    private bool _isLoadingBusy = false;
    public void UpdateKeys(List<string> keys) {
        UpdateKeysAsync(keys).GetAwaiter().GetResult();
    }
    public async Task UpdateKeysAsync(List<string> keys) {
        while (_isLoadingBusy) {
            await Task.Delay(100);
        }
        _isLoadingBusy = true;
        if (keys.Count == Target.Count) {
            for (int i = 0; i < keys.Count; i++) {
                await Target[i].SetKey(keys[i]);
            }
        }
        else if (keys.Count > Target.Count) {
            for (int i = 0; i < Target.Count; i++) {
                await Target[i].SetKey(keys[i]);
            }
            for (int i = Target.Count; i < keys.Count; i++) {
                TItem newSlot = new();
                await newSlot.SetKey(keys[i]);
                Target.Add(newSlot);
            }
        }
        else {
            for (int i = 0; i < keys.Count; i++) {
                await Target[i].SetKey(keys[i]);
            }
            while (Target.Count != keys.Count) {
                Target.RemoveAt(Target.Count - 1);
            }
        }
        KeyUpdated?.Invoke(this, EventArgs.Empty);
        _isLoadingBusy = false;
    }
    public async Task PageDown(int startIt, int amount) {
        if (!_isPagingBusy) {
            _isPagingBusy = true;
            int endIt = startIt + amount;
            startIt = Math.Clamp(startIt, 0, Target.Count);
            endIt = Math.Clamp(endIt, 0, Target.Count);
            for (int i = startIt; i < Target.Count && i < endIt; i++) {
                if (!Target[i].IsActive) {
                    await Target[i].Active();
                }
            }
            _isPagingBusy = false;
        }
    }
}