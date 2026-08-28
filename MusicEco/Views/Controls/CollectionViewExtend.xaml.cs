namespace MusicEco.Views.Controls;

using MusicEco.SourceGeneration;
using MusicEco.ViewModels;
using MusicEco.Views.Items;
using System.Collections.Specialized;
using System.Diagnostics;
using IEnumerable = System.Collections.IEnumerable;
#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif
/// <summary>
/// Since Native <see cref="CollectionView"/>.ItemsLayout does not change when reassign we need this class
/// </summary>
public partial class CollectionViewExtend: ContentView {
    private static readonly Type ThisType = typeof(CollectionViewExtend);
    [BindableAutoGen]
    public static readonly BindableProperty ItemsSourceProperty
        = Utility.Create<IEnumerable?>(ThisType, null,
            propertyChanged: (b, oldValue, newValue) => {
                var This = (CollectionViewExtend)b;
                var oldCollection = (IEnumerable?)oldValue;
                var newCollection = (IEnumerable?)newValue;
                This.SetCollection(oldCollection, newCollection);
            });
    [BindedProperty]
    public partial CollectionDisplayMode DisplayMode { get; set; }
    public static readonly BindableProperty DisplayModeProperty
        = Utility.Create<CollectionDisplayMode>(ThisType, CollectionDisplayMode.None,
            propertyChanged: (b, _, v) => {
                var This = (CollectionViewExtend)b;
                var value = (CollectionDisplayMode)v;
                This.ChangeDisplayMode(value);
            });
    public IList<CollectionViewPreset> ItemPresets { get; init; }
    public bool AllowDrop { get; set; }
    // true when DisplayMode use {Binding}, else when use direct value.
    // To handle UI initialization, after init, it is true in both cases.
    private bool _isBindedDisplayMode = true;
    private readonly DelayedDispatcher _dispatcher;
    public CollectionViewExtend() {
        this.AllowDrop = false;
        this.ItemPresets = [];
        InitializeComponent();
        CollectionView content = new();
        this.Content = content;
        this._dispatcher = new(this.Dispatcher, MusicEco.Config.ProgrammingDelay);
        this._cachedAction = new(RefreshStateInner);
    }
    private void SetCollection(IEnumerable? oldCollection, IEnumerable? newCollection) {
        if (!this._isBindedDisplayMode) {
            this.ChangeDisplayMode(this.DisplayMode);
        }
        if (this.Content is CollectionView view) {
            view.ItemsSource = newCollection;
        }
    }
    private void ChangeDisplayMode(CollectionDisplayMode displayMode) {
        CollectionViewPreset? selectedPreset = null;
        foreach (var preset in this.ItemPresets) {
            if (preset.DisplayMode == displayMode) {
                selectedPreset = preset;
                this._isBindedDisplayMode = true;
                break;
            }
        }
        if (selectedPreset == null) {
            this._isBindedDisplayMode = false;
            return;
            //throw new ArgumentOutOfRangeException(nameof(displayMode));
        }
        this._currentObservableCollection = null;
        this._firstVisbleindex = 0;
        this._lastVisibleIndex = 20;
        CollectionView view = new();
        // Old view will be discard by GC
        // So handler will be discard too
        // No need to unsubcribe
        view.Scrolled += this.View_Scrolled;
        view.PropertyChanged += this.View_PropertyChanged;
        if (this.AllowDrop) {
            var dropGestureRecognizer = new DropGestureRecognizer();
            dropGestureRecognizer.Drop += this.DropGestureRecognizer_Drop;
            view.GestureRecognizers.Add(dropGestureRecognizer);
        }
        if (selectedPreset.ItemsLayout != null) {
            view.ItemsLayout = selectedPreset.ItemsLayout;
        }
        if (selectedPreset.ItemTemplate != null) {
            view.ItemTemplate = selectedPreset.ItemTemplate;
        }
        view.ItemsSource = this.ItemsSource;
        this.Content = view;
    }
    private INotifyCollectionChanged? _currentObservableCollection;
    private IEnumerable? _enumerable;
    private int _firstVisbleindex = 0;
    private int _lastVisibleIndex = 20; // Tempory const value
    private void View_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(CollectionView.ItemsSource)) {
            if (sender is CollectionView collectionView) {
                this._currentObservableCollection?.CollectionChanged -= this.ObservableCollection_CollectionChanged;
                this._currentObservableCollection = null;
                this._enumerable = null;
                if (collectionView.ItemsSource is INotifyCollectionChanged observableCollection) {
                    observableCollection.CollectionChanged += this.ObservableCollection_CollectionChanged;
                    this._currentObservableCollection = observableCollection;
                }
                if (collectionView.ItemsSource is IEnumerable enumerable) {
                    this._enumerable = enumerable;
                }
                this.RefreshState();
            }
        }
    }

    private void ObservableCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        this.RefreshState();
    }

    private void View_Scrolled(object? sender, ItemsViewScrolledEventArgs e) {
        if (sender is CollectionView collectionView
            && collectionView.ItemsSource is IEnumerable enumerable) {
            int firstVisibleItemIndex;
            int lastVisibleItemIndex;
            if (collectionView.ItemsLayout is LinearItemsLayout) {
                // Linear use real indices
                firstVisibleItemIndex = e.FirstVisibleItemIndex;
                lastVisibleItemIndex = e.LastVisibleItemIndex;
            }
            else if (collectionView.ItemsLayout is GridItemsLayout a) {
#if WINDOWS
                // Windows Grid use virtualized indices
                if (collectionView.Handler?.PlatformView is ListViewBase nativeView
                    && nativeView.ItemsPanelRoot is ItemsWrapGrid panel) {
                    firstVisibleItemIndex = panel.FirstVisibleIndex;
                    lastVisibleItemIndex = panel.LastVisibleIndex;
                }
                else {
                    return;
                }
#else
                // Android use real indices
                firstVisibleItemIndex = e.FirstVisibleItemIndex;
                lastVisibleItemIndex = e.LastVisibleItemIndex;
#endif
            }
            else {
                return;
            }
            bool changed = false;
            if (this._firstVisbleindex != firstVisibleItemIndex) {
                this._firstVisbleindex = firstVisibleItemIndex;
                changed = true;
            }
            if (this._lastVisibleIndex != lastVisibleItemIndex) {
                this._lastVisibleIndex = lastVisibleItemIndex;
                changed = true;
            }
            if (changed) {
                this.RefreshState();
            }
        }
    }
    private void RefreshState() {
        this._dispatcher.Dispatch(this._cachedAction);
    }
    private readonly Action _cachedAction;
    private void RefreshStateInner() {
        if (this._firstVisbleindex != this._lastVisibleIndex
            && this._enumerable != null) {
            var enumerable = this._enumerable;
            int first = this._firstVisbleindex;
            int last = this._lastVisibleIndex;
            int index = 0;
            int visibleCount = 0;
            foreach (var item in enumerable) {
                if (item is IAnimationAware animationAware
                    && (index < first || index > last)) {
                    animationAware.IsVisibleInViewport = false;
                }
                index++;
            }
            index = 0;
            foreach (var item in enumerable) {
                if (item is IAnimationAware animationAware
                    && index >= first && index <= last) {
                    visibleCount++;
                    animationAware.IsVisibleInViewport = true;
                }
                index++;
            }
            Debug.WriteLine($"Visible: {visibleCount} || Hidden: {index - visibleCount} || {first} -> {last}");
        }
    }
    private void DropGestureRecognizer_Drop(object? sender, DropEventArgs e) {
        if (sender is CollectionView collectionView) {
            var items = collectionView.GetVisualTreeDescendants().OfType<DragItemFrame>();
            foreach (var item in items) {
                item.Reset();
            }
        }
    }
}