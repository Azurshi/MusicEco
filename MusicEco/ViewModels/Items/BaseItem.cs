using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEco.ViewModels.Items; 
public abstract class BaseItem: PropertyObject {
    public BaseItem() { }
    protected bool isActive = false;
    public bool IsActive => isActive;
    protected string? key;
    public string Key => key ?? string.Empty;
    /// <summary>
    /// Assign to true to make set <see cref="Key"/> affect when assign same value.
    /// </summary>
    protected bool RefreshOnKeyChanged = false;
    /// <summary>
    /// Auto call <see cref="OnActive"/> if <see cref="IsActive"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SetKey(string value) {
        if (key != value || RefreshOnKeyChanged) {
            key = value;
            OnPropertyChanged(nameof(key));
            if (isActive) await OnActive();
        }
    }
    /// <summary>
    /// Called on set <see cref="Key"/> or when call <see cref="Active"/>
    /// </summary>
    /// <returns></returns>
    protected abstract Task OnActive();
    /// <summary>
    /// For assigning <see cref="IsActive"/>, auto call <see cref="OnActive"/>
    /// </summary>
    /// <returns></returns>
    public virtual async Task Active() {
        isActive = true;
        await OnActive();
    }
}

public partial class BaseModifiableItem : BaseItem {
    private ViewMode _mode = ViewMode.Default;
    public ViewMode Mode {
        get => _mode;
        set {
            _mode = value;
            OnModeChanged();
        }
    }
    public virtual Task SaveData() { return Task.CompletedTask; }
    protected override Task OnActive() { return Task.CompletedTask; }
    public bool IsEditing => _mode == ViewMode.Edit;
    public bool IsViewing => _mode != ViewMode.Edit;
    protected void OnModeChanged() {
        OnPropertyChanged(nameof(Mode));
        OnPropertyChanged(nameof(IsEditing));
        OnPropertyChanged(nameof(IsViewing));
    }
    [RelayCommand]
    private void EnableEditMode() {
        Mode = ViewMode.Edit;
    }
    [RelayCommand]
    private async Task ConfirmEdit() {
        Mode = ViewMode.Confirm;
        await SaveData();
    }
    [RelayCommand]
    private void CancelEdit() {
        Mode = ViewMode.Cancel;
    }
}


public partial class BaseDraggableItem : BaseItem, IDraggable {
    protected override Task OnActive() { return Task.CompletedTask; }
    protected bool _isDraggable = false;
    public bool IsDraggable {
        get => _isDraggable;
        set {
            _isDraggable = value;
            OnPropertyChanged();
        }
    }
    protected bool _isDragged = false;
    public bool IsDragged {
        get => _isDragged;
        set {
            _isDragged = value;
            OnPropertyChanged();
        }
    }
    protected bool _isDraggedOver = false;
    public bool IsDraggedOver {
        get => _isDraggedOver;
        set {
            _isDraggedOver = value;
            OnPropertyChanged();
        }
    }
}

public partial class BaseModifiableDraggableItem : BaseModifiableItem, IDraggable {
    protected bool _isDraggable = false;
    public bool IsDraggable {
        get => _isDraggable;
        set {
            _isDraggable = value;
            OnPropertyChanged();
        }
    }
    protected bool _isDragged = false;
    public bool IsDragged {
        get => _isDragged;
        set {
            _isDragged = value;
            OnPropertyChanged();
        }
    }
    protected bool _isDraggedOver = false;
    public bool IsDraggedOver {
        get => _isDraggedOver;
        set {
            _isDraggedOver = value;
            OnPropertyChanged();
        }
    }
}