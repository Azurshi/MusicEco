using MusicEco.ViewModels;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MusicEco.Views.Widgets;

public partial class DataList : CollectionView
{
    public class LoadMoreItemEventArgs(int lastIndex, int amount) : EventArgs {
        public int LastIndex { get; set; } = lastIndex;
        public int Amount { get; set; } = amount;
    }
    #region Binding
    private static readonly Type ThisType = typeof(DataList);
    public static readonly BindableProperty PreloadAmountProperty =
        Utility.Create<int>(ThisType);
    public int PreloadAmount {
        get => (int)GetValue(PreloadAmountProperty);
        set => SetValue(PreloadAmountProperty, value);
    }
    public ICommand LoadMoreItemCommand {
        get => (ICommand)GetValue(LoadMoreItemCommandProperty);
        set => SetValue(LoadMoreItemCommandProperty, value);
    }
    public static readonly BindableProperty LoadMoreItemCommandProperty =
        Utility.Create<ICommand>(ThisType);
    #endregion
    public DataList()
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
            double itemHeight = (double)resources["ListItemSize"];
            int lastVisibleRow = (int)((e.VerticalOffset + AppShell.ScreenHeight) / itemHeight);
            LoadMoreItemEventArgs args = new(lastVisibleRow, PreloadAmount);
            LoadMoreItemCommand?.Execute(args);
        }
    }
    public void ToTop() {
        this.ScrollTo(0, animate: false);
        this.lastScrolled = 0;
    }
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = "") {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(this.ItemsSource)) {
            IEnumerable data = this.ItemsSource;
            if (data is INotifyUpdated observableData) {
                observableData.CollectionUpdated += this.ToTop;
            }
        }
    }
    #endregion
}