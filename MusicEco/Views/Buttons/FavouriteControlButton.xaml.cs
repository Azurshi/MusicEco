using MusicEco.Core.Services;
using MusicEco.Services;
using MusicEco.SourceGeneration;
using MusicEco.Views.Commands;
using System.Windows.Input;

namespace MusicEco.Views.Buttons;

public partial class FavouriteControlButton: ContentView {
    private static readonly Type ThisType = typeof(FavouriteControlButton);
    [BindableAutoGen]
    public static readonly BindableProperty CommandProperty
        = Utility.Create<ICommand?>(ThisType);
    [BindedProperty]
    public partial string ResourcePath { get; set; }
    public static readonly BindableProperty ResourcePathProperty
        = Utility.Create<string>(ThisType, string.Empty,
            propertyChanged: (b, _, v) => {
                var This = (FavouriteControlButton)b;
                var value = (string)v;
                This.ImageLabel.ResourcePath = value;
            });
    [BindedProperty]
    public partial Color? ActiveTintColor { get; set; }
    public static readonly BindableProperty ActiveTintColorProperty
        = Utility.Create<Color?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (FavouriteControlButton)b;
                var value = (Color?)v;
                This.RefreshDisplay();
            });
    [BindedProperty]
    public partial Color? InactiveTintColor { get; set; }
    public static readonly BindableProperty InactiveTintColorProperty
        = Utility.Create<Color?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (FavouriteControlButton)b;
                var value = (Color?)v;
                This.RefreshDisplay();
            });
    private bool _isFavourite = false;
    private readonly IFavouriteService _favouriteService;
    private readonly IQueueService _queueServce;
    private void RefreshDisplay() {
        if (this._isFavourite) {
            this.ImageLabel.TintColor = this.ActiveTintColor;
        }
        else {
            this.ImageLabel.TintColor = this.InactiveTintColor;
        }
    }
    public FavouriteControlButton() {
        InitializeComponent();
        this._favouriteService = AppLifeCycle.Provider.GetRequiredService<IFavouriteService>();
        this._queueServce = AppLifeCycle.Provider.GetRequiredService<IQueueService>();

        // Lifetime event
        this._favouriteService.ItemsChanged += this.FavouriteService_ItemsChanged;
        this._queueServce.CurrentChanged += this.QueueServce_CurrentChanged;
        this.Loaded += this.FavouriteControlButton_Loaded;
    }

    private async void FavouriteControlButton_Loaded(object? sender, EventArgs e) {
        await RefreshState();
    }

    private async void QueueServce_CurrentChanged(object? sender, EventArgs e) {
        await RefreshState();
    }

    private async Task RefreshState() {
        var currentQueue = await this._queueServce.GetCurrent();
        if (currentQueue != null) {
            var currentAudio = currentQueue.Current;
            if (currentAudio != null) {
                var hash = currentAudio.Hash;
                if (await this._favouriteService.IsFavourite(hash)) {
                    this._isFavourite = true;
                }
                else {
                    this._isFavourite = false;
                }
                this.RefreshDisplay();
            }
        }
    }
    private async void FavouriteService_ItemsChanged(object? sender, EventArgs e) {
        await RefreshState();
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e) {
        Scale = 0.9;
        BackgroundColor = Colors.Transparent;
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

    private async void OnTapped(object? sender, TappedEventArgs e) {
        var currentQueue = await this._queueServce.GetCurrent();
        if (currentQueue != null) {
            var currentAudio = currentQueue.Current;
            if (currentAudio != null) {
                var hash = currentAudio.Hash;
                if (this._isFavourite) {
                    await this._favouriteService.RemoveFavourite(hash, this);
                }
                else {
                    await this._favouriteService.AddFavourite(hash, this);
                }
            }
        }
    }
}