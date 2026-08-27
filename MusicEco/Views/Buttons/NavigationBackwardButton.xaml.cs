using MusicEco.Core.Types;
using MusicEco.Services;
using MusicEco.SourceGeneration;

namespace MusicEco.Views.Buttons;

public partial class NavigationBackwardButton: ContentView, IDisposable {
    private static readonly Type ThisType = typeof(NavigationBackwardButton);
    [BindedProperty]
    public partial double SizeRequest { get; set; }
    public static readonly BindableProperty SizeRequestProperty
        = Utility.Create<double>(ThisType, 64.0,
            propertyChanged: (b, _, v) => {
                var This = (NavigationBackwardButton)b;
                var value = (double)v;
                This.InnerButtonSize = value - Utility.GetResource<double>("FrameMarginSize");
            });
    [BindedProperty]
    public partial double InnerButtonSize { get; set; }
    public static readonly BindableProperty InnerButtonSizeProperty
        = Utility.Create<double>(ThisType, 64.0);
    private readonly NavigationStack _stack;
    public NavigationBackwardButton() {
        InitializeComponent();
        this._stack = AppLifeCycle.Provider.GetRequiredService<NavigationStack>();
        this.InnerButton.Command = new SyncCommandExtend(this._stack.PreviousPage, this._stack.CanNavigateToPreviousPage);
        this._stack.RouteChanged += this.Stack_RouteChanged;
    }
    ~NavigationBackwardButton() {
        this._stack.RouteChanged -= this.Stack_RouteChanged;
    }
    private void Stack_RouteChanged(object? sender, PageRoute e) {
        if (this.InnerButton.Command is SyncCommandExtend command) {
            command.NotifyCanExecute();
            this.InnerButton.BackgroundColor = Colors.Transparent;
        }
    }
    public void Dispose() {
        this._stack.RouteChanged -= this.Stack_RouteChanged;
        GC.SuppressFinalize(this);
    }
}