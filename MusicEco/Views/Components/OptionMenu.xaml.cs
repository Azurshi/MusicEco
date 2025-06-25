using MusicEco.ViewModels;

namespace MusicEco.Views.Components;

public partial class OptionMenu : ContentView
{
    #region Binding
    private static readonly Type ThisType = typeof(OptionMenu);
    public static readonly BindableProperty MainContentProperty =
        Utility.Create<View>(ThisType, OnMainContentPropertyChanged);
    private static void OnMainContentPropertyChanged(BindableObject obj, object oldValue, object newValue) {
        OptionMenu This = (OptionMenu)obj;
        This.MainContentPresenter.Content = (View)newValue;
    }
    public View MainContent {
        get => (View)GetValue(MainContentProperty);
        set => SetValue(MainContentProperty, value);
    }
    public static readonly BindableProperty MainBindingContextProperty =
        Utility.Create<PropertyObject>(ThisType, OnMainBindingContextPropertyChanged);
    private static void OnMainBindingContextPropertyChanged(BindableObject obj, object oldValue, object newValue) {
        OptionMenu This = (OptionMenu)obj;
        This.MainContentPresenter.Content.BindingContext = (PropertyObject)newValue;
    }
    public PropertyObject MainBindingContext {
        get => (PropertyObject)GetValue(MainBindingContextProperty);
        set => SetValue(MainBindingContextProperty, value);
    }
    #endregion
    public OptionMenu() {
        InitializeComponent();
    }
    public void SetBindingContext(object bindingContext, object? parentBindingContext = null) {
        this.BindingContext = parentBindingContext;
        this.MainContentPresenter.Content.BindingContext = bindingContext;
    }
}