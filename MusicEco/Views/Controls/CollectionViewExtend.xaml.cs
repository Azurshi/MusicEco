namespace MusicEco.Views.Controls;

using MusicEco.SourceGeneration;
using MusicEco.ViewModels;
using IEnumerable = System.Collections.IEnumerable;
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
    [BindableAutoGen]
    public static readonly BindableProperty DisplayModeProperty
        = Utility.Create<CollectionDisplayMode>(ThisType, CollectionDisplayMode.None,
            propertyChanged: (b, _, v) => {
                var This = (CollectionViewExtend)b;
                var value = (CollectionDisplayMode)v;
                This.ChangeDisplayMode(value);
            });
    public IList<CollectionViewPreset> ItemPresets { get; init; }
    public CollectionViewExtend() {
        InitializeComponent();
        this.ItemPresets = [];
        CollectionView content = new();
        this.Content = content;
    }
    private void SetCollection(IEnumerable? oldCollection, IEnumerable? newCollection) {
        if (this.Content is CollectionView view) {
            view.ItemsSource = newCollection;
        }
    }
    private void ChangeDisplayMode(CollectionDisplayMode displayMode) {
        CollectionViewPreset? selectedPreset = null;
        foreach(var preset in this.ItemPresets) {
            if (preset.DisplayMode == displayMode) {
                selectedPreset = preset;
                break;
            }
        }
        if (selectedPreset == null) {
            throw new ArgumentOutOfRangeException(nameof(displayMode));
        }
        CollectionView view = new();
        if (selectedPreset.ItemsLayout != null) {
            view.ItemsLayout = selectedPreset.ItemsLayout;
        }
        if (selectedPreset.ItemTemplate != null) {
            view.ItemTemplate = selectedPreset.ItemTemplate;
        }
        view.ItemsSource = this.ItemsSource;
        this.Content = view;
    }
}