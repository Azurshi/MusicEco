using CommunityToolkit.Mvvm.Input;
using MusicEco.ViewModels.Items;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.ViewModels;
public partial class DragList : PropertyObject {
    public virtual ObservableCollection<BaseItem> Data { get; } = [];
    public virtual async Task OnDropCompleted(int index, string key) {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    [RelayCommand]
    private void ItemDragged(IDraggable? draggedItem) {
        if (draggedItem == null) return;
        //Debug.WriteLine($"Start drag {draggedItem.Key}");
        draggedItem.IsDragged = true;
    }
    [RelayCommand]
    private void ItemDraggedOver(IDraggable? dragOverItem) {
        if (dragOverItem == null) return;
        //Debug.WriteLine($"Drag over {dragOverItem.Key}");
        var draggedItem = Data.Where(i => ((IDraggable)i).IsDragged).FirstOrDefault();
        if (draggedItem == null || dragOverItem.Key != draggedItem.Key) {
            dragOverItem.IsDraggedOver = true;
        }
    }
    [RelayCommand]
    private void ItemDragLeave(IDraggable? dragLeaveItem) {
        if (dragLeaveItem == null) return;
        //Debug.WriteLine($"Drag leave {dragLeaveItem.Key}");
        dragLeaveItem.IsDraggedOver = false;
    }

    [RelayCommand]
    private async Task ItemDropped() {
        var itemToMove = Data.Where(i => ((IDraggable)i).IsDragged).FirstOrDefault();
        var itemToInsertBefore = Data.Where(i => ((IDraggable)i).IsDraggedOver).FirstOrDefault();
        if (itemToMove == null || itemToInsertBefore == null || itemToMove == itemToInsertBefore) {
            Debug.WriteLine("Invalid drop");
            return;
        }
        //Debug.WriteLine(itemToMove.Key + " -> " + itemToInsertBefore.Key);
        ((IDraggable)itemToInsertBefore).IsDraggedOver = false;
        ((IDraggable)itemToMove).IsDragged = false;
        Data.Remove(itemToMove);
        int index = Data.IndexOf(itemToInsertBefore);
        Data.Insert(index + 1, itemToMove);
        await OnDropCompleted(index + 1, itemToMove.Key);
    }
}
