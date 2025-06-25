using MusicEco.ViewModels;
using MusicEco.ViewModels.Components;
using MusicEco.Views.Components;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.DetailPages;

public partial class BaseUserDetailPage : ContentPage, IBasePage, IServiceAccess {
    #region Binding
    private static readonly Type ThisType = typeof(BaseUserDetailPage);
    public static readonly BindableProperty MainContentProperty =
        Utility.Create<View>(ThisType, OnMainContentPropertyChanged);
    private static void OnMainContentPropertyChanged(BindableObject obj, object oldValue, object newValue) {
        BaseUserDetailPage This = (BaseUserDetailPage)obj;
        This.MainContentPresenter.Content = (View)newValue;
    }
    public View MainContent {
        get => (View)GetValue(MainContentProperty);
        set => SetValue(MainContentProperty, value);
    }
    public static readonly BindableProperty MainBindingContextProperty =
        Utility.Create<PropertyObject>(ThisType, OnMainBindingContextPropertyChanged);
    private static void OnMainBindingContextPropertyChanged(BindableObject obj, object oldValue, object newValue) {
        BaseUserDetailPage This = (BaseUserDetailPage)obj;
        This.MainContentPresenter.Content.BindingContext = (PropertyObject)newValue;
    }
    public PropertyObject MainBindingContext {
        get => (PropertyObject)GetValue(MainBindingContextProperty);
        set => SetValue(MainBindingContextProperty, value);
    }

    Overlay IBasePage.PageOverlay => PageOverlay;
    #endregion
    public BaseUserDetailPage()
	{
		InitializeComponent();
    }
    private async void BackButton_Clicked(object sender, EventArgs e) {
        await MusicEco.ViewModels.Utility.GoToAsync();
    }
}