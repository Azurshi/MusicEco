using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.ViewModels.Base;
using System.Collections.ObjectModel;

namespace MusicEco.Views.Widgets;

public partial class DataGrid : ContentView {
    #region Binding
    public static readonly BindableProperty ItemsSourceProperty =
    BindableProperty.Create(
        nameof(ItemsSource),
        typeof(ObservableCollection<BaseSlot>),
        typeof(DataGrid),
        propertyChanged: OnItemsSourceChanged
    );
    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue) {
        ((DataGrid)bindable).CollectionContainer.ItemsSource = (ObservableCollection<BaseSlot>)newValue;
    }
    public ObservableCollection<BaseSlot> ItemsSource {
        get => (ObservableCollection<BaseSlot>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }
    public static readonly BindableProperty ColumnsCountProperty =
        BindableProperty.Create(
            nameof(ColumnsCount),
            typeof(int),
            typeof(DataGrid),
            propertyChanged: OnNumOfColumnsChanged
        );
    private static void OnNumOfColumnsChanged(BindableObject bindable, object oldValue, object newValue) {
        ((DataGrid)bindable).ItemLayout.Span = (int)newValue;
    }
    public int ColumnsCount {
        get => (int)GetValue(ColumnsCountProperty);
        set => SetValue(ColumnsCountProperty, value);
    }
    public DataTemplate ItemTemplate {
        get => CollectionContainer.ItemTemplate;
        set => CollectionContainer.ItemTemplate = value;
    }
    #endregion
    #region Resource
    public DataGrid() {
        InitializeComponent();
    }
    #endregion
    #region SlotSignal
    public event EventHandler<StringEventArgs>? SlotSelected;
    public void SlotSelectedInvoke(string key) {
        SlotSelected?.Invoke(this, new(key));
    }
    #endregion
    #region Incremental
    private double _yScrolled = 0;
    public event EventHandler<IntEventArgs>? LoadMoreItemRequest;
    private void ScrollWrapper_Scrolled(object sender, ScrolledEventArgs e) {
        _yScrolled = e.ScrollY;
        LoadMoreItemRequest?.Invoke(this.Content, new((int)(_yScrolled / Common.Value.UI.RowHeight / Setting.GridHeightMulti)));
    }
    #endregion
}