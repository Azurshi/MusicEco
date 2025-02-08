using MusicEco.Common.Events;
using MusicEco.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MusicEco.Views.Widgets;

public partial class DataList : ContentView
{
    #region Binding
    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ObservableCollection<BaseSlot>),
            typeof(DataList),
            propertyChanged: OnItemsSourceChanged
        );
    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue) {
        ((DataList)bindable).CollectionContainer.ItemsSource = (ObservableCollection<BaseSlot>)newValue;
    }
    public ObservableCollection<BaseSlot> ItemsSource {
        get => (ObservableCollection<BaseSlot>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    public DataTemplate ItemTemplate {
        get => CollectionContainer.ItemTemplate;
        set => CollectionContainer.ItemTemplate = value;
    }
    #endregion
    #region SlotSignal
    public event EventHandler<StringEventArgs>? SlotSelected;
    public void SlotSelectedInvoke(string key) {
        SlotSelected?.Invoke(this, new(key));
    }
    public event EventHandler<StringEventArgs>? SlotOptionSelected;
    public void SlotOptionInvoke(string key) {
        SlotOptionSelected?.Invoke(this, new(key));
    }
    //public event EventHandler? RequestUpdate;
    //public void RequestUpdateInvoke() {
    //    RequestUpdate?.Invoke(this, EventArgs.Empty);
    //}
    #endregion
    public event EventHandler<IntEventArgs>? LoadMoreItemRequest;
    private double _yScrolled = 0;
    public DataList() {
        InitializeComponent();
    }
    private void ScrollWrapper_Scrolled(object sender, ScrolledEventArgs e) {
        _yScrolled = e.ScrollY;
        LoadMoreItemRequest?.Invoke(this.Content, new((int)(_yScrolled / Common.Value.UI.RowHeight)));
    }
    #region Drag&Drop
    //private BaseSlot? GetItemData(string key) {
    //    return ItemsSource.FirstOrDefault(p => p.Key == key);
    //}
    //private VisualElement InstanciateNew(string slotKey) {
    //    throw new NotImplementedException();
    //    BaseSlot slotData = ItemsSource[0].Produce();
    //    slotData.Key = slotKey;
    //    //DataTemplateSelector itemTemplate;
    //    //if (slotData is SongSlot) {
    //    //    itemTemplate = (DataTemplateSelector)this.Resources["SongSlotSelector"];
    //    //} else {
    //    //    itemTemplate = (DataTemplateSelector)this.Resources["TextSlotSelector"];
    //    //}
    //    DataTemplateSelector itemTemplate = (DataTemplateSelector)this.Resources["UniversalSlotSelector"];
    //    VisualElement result = (VisualElement)itemTemplate.SelectTemplate(slotData, this).CreateContent();
    //    result.BindingContext = slotData;
    //    slotData.Active();
    //    return result;
    //}
    //private BaseSlot? GetItemBasedOnPosition(double gridAbsoluteY) {
    //    int index = (int)Math.Floor(gridAbsoluteY / (BaseSlot.GlobalSlotHeight + Misc.ToPixels(ItemLayout.ItemSpacing) * 2));
    //    if (index < 0 || index >= ItemsSource.Count) {
    //        return null;
    //    }
    //    else {
    //        return ItemsSource[index];
    //    }
    //}
    //private string lastDragOverItemKey = Common.Value.Null.String;
    //private VisualElement? draggedItem;
    //private BaseSlot? originalItemData;
    //private double yOffset = 0;
    //public event EventHandler? RequestUpdate;
    //public event EventHandler<StringChangeEventArgs>? ListRearrangeFunction;
    //private void OnPanStated(object? sender, EventArgs e) {
    //    if (sender is MoveLabel moveLabel) {
    //        BaseSlot baseSlot = (BaseSlot)moveLabel.BindingContext;
    //        string key = baseSlot.Key;

    //        draggedItem = InstanciateNew(key);
    //        originalItemData = GetItemData(key);
    //        originalItemData!.ContentVisible = false;
    //        yOffset = ItemsSource.IndexOf(originalItemData) * (Misc.ToPixels(ItemLayout.ItemSpacing) * 2 + originalItemData.SlotHeight);
    //        if (Overlay != null) {
    //            Overlay.Children.Add(draggedItem);
    //            Overlay.InputTransparent = false;
    //        }
    //        else {
    //            Debug.WriteLine("Overlay is null");
    //        }
    //    }
    //}
    //private void OnPanPositionChange(object? sender, Event.Vector2EventArgs e) {
    //    float y = e.Value.Y;
    //    AbsoluteLayout.SetLayoutBounds(draggedItem, new Rect(0, y + yOffset - yScrolled, Overlay.Width, originalItemData!.SlotHeight));
    //    BaseSlot? newItem = GetItemBasedOnPosition(y + yOffset);

    //    if (newItem != null && newItem.Key != lastDragOverItemKey) {
    //        var oldItemData = GetItemData(lastDragOverItemKey);
    //        if (oldItemData != null) oldItemData.ContentVisible = true;
    //        lastDragOverItemKey = newItem.Key;
    //        if (ListRearrangeFunction != null) {
    //            Value.System.BlockUpdate = true;
    //            ListRearrangeFunction?.Invoke(this.Content, new(originalItemData!.Key, newItem.Key));
    //            Value.System.BlockUpdate = false;
    //        }
    //        else Debug.WriteLine("Rearrange function is null");
    //        string oldId = originalItemData!.Key;
    //        RequestUpdate?.Invoke(this.Content, EventArgs.Empty);
    //        originalItemData = GetItemData(oldId);
    //        originalItemData!.ContentVisible = false;
    //    }

    //}
    //private void OnPanCompleted(object? sender, EventArgs e) {
    //    Overlay!.InputTransparent = true;
    //    Overlay.Children.Remove(draggedItem);
    //    draggedItem = null;
    //    if (originalItemData != null) {
    //        originalItemData.ContentVisible = true;
    //    }
    //    originalItemData = null;
    //}
    #endregion
}