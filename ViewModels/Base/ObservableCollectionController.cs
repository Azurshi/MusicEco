using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEco.ViewModels.Base;
public class ObservableCollectionController<TSlot>(ObservableCollection<BaseSlot> target, int frameAmount) where TSlot: BaseSlot, new() {
    public event EventHandler? KeyUpdated;
    public readonly ObservableCollection<BaseSlot> Target = target;
    public readonly int FrameAmount = frameAmount;
    private bool _isPagingBusy = false;
    private bool _isLoadingBusy = false;

    public void UpdateKeys(List<string> keys) {
        UpdateKeysAsync(keys).GetAwaiter().GetResult();
    }
    public async Task UpdateKeysAsync(List<string> keys) {
        while (_isLoadingBusy) {
            await Task.Delay(Common.Value.TimeSpan.AsyncShortDelay);
        }
        _isLoadingBusy = true;
        if (keys.Count == Target.Count) {
            for (int i = 0; i < keys.Count; i++) {
                Target[i].Key = keys[i];
            }
        }
        else if (keys.Count > Target.Count) {
            for (int i = 0; i < Target.Count; i++) {
                Target[i].Key = keys[i];
            }
            for (int i = Target.Count; i < keys.Count; i++) {
                TSlot newSlot = new();
                newSlot.Key = keys[i];
                Target.Add(newSlot);
            }
        }
        else {
            for (int i = 0; i < keys.Count; i++) {
                Target[i].Key = keys[i];
            }
            while (Target.Count != keys.Count) {
                Target.RemoveAt(Target.Count - 1);
            }
        }
        await PageDown(0);
        KeyUpdated?.Invoke(this, EventArgs.Empty);
        _isLoadingBusy = false;
    }
    public async Task PageDown(int startIt) {
        if (!_isPagingBusy) {
            _isPagingBusy = true;
            int endIt = startIt + FrameAmount;
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