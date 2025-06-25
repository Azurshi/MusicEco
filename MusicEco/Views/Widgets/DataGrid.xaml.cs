using System.Diagnostics;
using System.Windows.Input;

namespace MusicEco.Views.Widgets;


public partial class DataGrid : CollectionView
{
    public class LoadMoreItemEventArgs(int lastActivatedRow, int rowAmount, int columnCount) : EventArgs {
        public int LastActivatedRow { get; set; } = lastActivatedRow;
        public int RowAmount { get; set; } = rowAmount;
        public int ColumnCount { get; set; } = columnCount;
    }
    #region Binding
    private static readonly Type ThisType = typeof(DataGrid);
    public static readonly BindableProperty ColumnsCountProperty =
        Utility.Create<int>(ThisType, propertyChanged:
            (b, _, v) => {
                ((DataGrid)b).ItemLayout.Span = (int)v;
        });
    public int ColumnsCount {
        get => (int)GetValue(ColumnsCountProperty);
        set => SetValue(ColumnsCountProperty, value);
    }
    public static readonly BindableProperty RowPreloadAmountProperty =
        Utility.Create<int>(ThisType);
    public int RowPreloadAmount {
        get => (int)GetValue(RowPreloadAmountProperty);
        set => SetValue(RowPreloadAmountProperty, value);
    }
    public ICommand LoadMoreItemCommand {
        get => (ICommand)GetValue(LoadMoreItemCommandProperty);
        set => SetValue(LoadMoreItemCommandProperty, value);
    }
    public static readonly BindableProperty LoadMoreItemCommandProperty =
        Utility.Create<ICommand>(ThisType);
    #endregion
    public DataGrid()
	{
		InitializeComponent();
	}
    #region Incremental
    //public event EventHandler<LoadMoreItemEventArgs>? LoadMoreItemRequest;
    private double lastScrolled = 0;
    /// <summary>
    /// Doesn't count spacing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e) {
        if (lastScrolled < e.VerticalOffset) {
            lastScrolled = e.VerticalOffset;
            ResourceDictionary resources = Application.Current!.Resources;
            double itemHeight = (double)resources["GridItemSize"];
            int lastVisibleRow = (int)((e.VerticalOffset + AppShell.ScreenHeight) / itemHeight);
            LoadMoreItemEventArgs args = new(lastVisibleRow, RowPreloadAmount, ColumnsCount);
            LoadMoreItemCommand?.Execute(args);
        }
    }
    #endregion
}