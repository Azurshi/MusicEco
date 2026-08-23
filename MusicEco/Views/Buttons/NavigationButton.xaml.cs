using MusicEco.Core;
using MusicEco.Services;
using MusicEco.SourceGeneration;
using MusicEco.Views.Controls;

namespace MusicEco.Views.Buttons;

public partial class NavigationButton: ContentView {
    private static readonly Type ThisType = typeof(NavigationButton);
    private static bool ComputeActive(PageRoute? currentRoute, PageRoute buttonRoute) {
        if (currentRoute == null) {
            return false;
        }
        else {
            return currentRoute.Route.StartsWith(buttonRoute.Route);
        }
    }
    [BindedProperty]
    public partial string ResourcePath { get; set; }
    public static readonly BindableProperty ResourcePathProperty
        = Utility.Create<string>(ThisType, string.Empty,
            propertyChanged: (b, _, v) => {
                var This = (NavigationButton)b;
                var value = (string)v;
                This.ImageLabel.ResourcePath = value;
            });
    [BindedProperty]
    public partial PageRoute PageRoute { get; set; }
    public static readonly BindableProperty PageRouteProperty
        = Utility.Create<PageRoute>(ThisType, PageRoute.None,
            propertyChanged: (b, _, v) => {
                var This = (NavigationButton)b;
                var value = (PageRoute)v;
                This.IsActivate = ComputeActive(This._stack.CurrentRoute, value);
            });
    [BindedProperty]
    public partial Color? ActiveTintColor { get; set; }
    public static readonly BindableProperty ActiveTintColorProperty
        = Utility.Create<Color?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (NavigationButton)b;
                var value = (Color?)v;
                This.RefreshState();
            });
    [BindedProperty]
    public partial Color? InactiveTintColor { get; set; }
    public static readonly BindableProperty InactiveTintColorProperty
        = Utility.Create<Color?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (NavigationButton)b;
                var value = (Color?)v;
                This.RefreshState();
            });
    private bool _isActivate = false;
    private bool IsActivate {
        get => _isActivate;
        set {
            if (this._isActivate != value) {
                this._isActivate = value;
                this.RefreshState();
            }
        }
    }
    private void RefreshState() {
        if (this._isActivate) {
            this.ImageLabel.TintColor = this.ActiveTintColor;
        }
        else {
            this.ImageLabel.TintColor = this.InactiveTintColor;
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
        if (ComputeActive(e, this.PageRoute)) {
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
        BackgroundColor = DynamicColors.ButtonHighlightColor;
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e) {
        BackgroundColor = DynamicColors.ButtonHighlightColor;
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
    public void Dispose() {
        //Debug.WriteLine("Detach navigation event");
        this._stack.RouteChanged -= NavigationButton_RouteChanged;
    }
}