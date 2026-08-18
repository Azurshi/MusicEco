using MusicEco.Core;
using MusicEco.Services;
using MusicEco.SourceGeneration;
using System.Windows.Input;

namespace MusicEco.Views.Buttons;

public partial class NavigationButton: Grid {
    private static readonly Type ThisType = typeof(NavigationButton);
    [BindedProperty]
    public partial PageRoute PageRoute { get; set; }
    public static readonly BindableProperty PageRouteProperty 
        = Utility.Create<PageRoute>(ThisType, PageRoute.None,
            propertyChanged: (b, _, v) => {
                var This = (NavigationButton)b;
                var value = (PageRoute)v;
                This.IsActivate = This._stack.CurrentRoute == value;
            });
    [BindedProperty]
    public partial ImageSource? ActivateImageSource { get; set; }
    public static readonly BindableProperty ActivateImageSourceProperty
        = Utility.Create<ImageSource?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (NavigationButton)b;
                var value = (ImageSource?)v;
                This.ActivateImage.Source = value;
            });
    [BindedProperty]
    public partial ImageSource? DeactivateImageSource { get; set; }
    public static readonly BindableProperty DeactivateImageSourceProperty
        = Utility.Create<ImageSource?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (NavigationButton)b;
                var value = (ImageSource?)v;
                This.DeactivateImage.Source = value;
            });
    private bool _isActivate = false;
    private bool IsActivate {
        get => _isActivate;
        set {
            if (this._isActivate != value) {
                this._isActivate = value;
                this.ActivateImage.IsVisible = this._isActivate;
                this.DeactivateImage.IsVisible = !this._isActivate;
            }
        }
    }
    private readonly NavigationStack _stack;
    public NavigationButton() {
        InitializeComponent();
        this._stack = AppLifeCycle.Provider.GetRequiredService<NavigationStack>();
        this._stack.RouteChanged += this.NavigationButton_RouteChanged;
        this.IsActivate = false;
    }

    private void NavigationButton_RouteChanged(object? sender, PageRoute e) {
        bool isActivate = false;
        if (e.Route.StartsWith(this.PageRoute.Route)) {
            isActivate = true;
        }
        this.IsActivate = isActivate;
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e) {
        if (!this.IsActivate) {
            Scale = 0.9;
            BackgroundColor = Colors.Transparent;
        }
    }

    private void OnPointerReleased(object? sender, PointerEventArgs e) {
        Scale = 1.0;
        BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e) {
        BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void OnPointerExited(object? sender, PointerEventArgs e) {
        BackgroundColor = Colors.Transparent;
    }

    private void OnTapped(object? sender, TappedEventArgs e) {
        if (!this.IsActivate) {
            var currentRoute = this._stack.CurrentRoute;
            if (currentRoute != null) {
                var navigateEventArgs = new NavigateEventArgs(this, currentRoute, this.PageRoute);
                EventSystem.Publish(this, navigateEventArgs);
            }
        }
    }
}