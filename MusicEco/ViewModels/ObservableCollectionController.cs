using MusicEco.ViewModels.Items;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels;
public interface INotifyUpdated {
    public event VoidEventHandler? CollectionUpdated;
    public abstract void InvokeUpdated();
}
public class ObservableCollectionExtend<T> : ObservableCollection<T>, INotifyUpdated {
    public event VoidEventHandler? CollectionUpdated;
    public void InvokeUpdated() {
        this.CollectionUpdated?.Invoke();
    }
}
public class ObservableCollectionController<TItem>(ObservableCollectionExtend<BaseItem> target) where TItem : BaseItem, new() {
    public readonly ObservableCollectionExtend<BaseItem> Target = target;
    private bool _isLoadingBusy = false;
    private int scheduledIndex = 0;
    private int loadedIndex = 0;
    public void UpdateKeys(List<string> keys) {
        UpdateKeysAsync(keys).GetAwaiter().GetResult();
    }
    public async Task UpdateKeysAsync(List<string> keys, bool forceUpdate = false) {
        while (_isLoadingBusy) {
            await Task.Delay(100);
        }
        if (!forceUpdate) {
            bool same = keys.Count == Target.Count;
            if (same) {
                for(int i=0; i<keys.Count; i++) {
                    if (keys[i] != Target[i].Key) {
                        same = false;
                        break;
                    }
                }
            }
            if (same) {
                return;
            }
        }
        _isLoadingBusy = true;
        scheduledIndex = 0;
        loadedIndex = 0;
        if (keys.Count == Target.Count) {
            for (int i = 0; i < keys.Count; i++) {
                Target[i].DeActive();
                await Target[i].SetKey(keys[i]);
            }
        }
        else if (keys.Count > Target.Count) {
            for (int i = 0; i < Target.Count; i++) {
                Target[i].DeActive();
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
                Target[i].DeActive();
                await Target[i].SetKey(keys[i]);
            }
            while (Target.Count != keys.Count) {
                Target.RemoveAt(Target.Count - 1);
            }
        }
        Target.InvokeUpdated();
        _isLoadingBusy = false;
    }
    public async Task PageDown(int startIndex, int amount) {
        if (Target.Count <= 0) return;
        //Debug.WriteLine(startIndex);
        int target = startIndex + amount;
        target = Math.Clamp(target, 0, Target.Count-1);
        if (scheduledIndex < target) {
            //Debug.WriteLine($"Target {target} | Loading: {loadedIndex} / {scheduledIndex}");
            scheduledIndex = target;
            for(int i=loadedIndex; i<=scheduledIndex; i++) {
                if (!Target[i].IsActive) {
                    await Target[i].Active();
                    if (Target[i].IsActive) {
                        loadedIndex = i;
                    }
                }
            }
        }
    }
}