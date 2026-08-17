using MusicEco.Core.Services;
using MusicEco.Services;
using MusicEco.SourceGeneration;
using MusicEco.Views.Commands;
using System.Windows.Input;

namespace MusicEco.Views.Buttons;

public partial class FavouriteControlButton: ContentView {
    private static readonly Type ThisType = typeof(FavouriteControlButton);
    [BindableAutoGen<ICommand>(IsNullable = true)]
    public static readonly BindableProperty CommandProperty
        = Utility.Create<ICommand?>(ThisType);
    public static readonly BindableProperty ActivateImageSourceProperty
        = Utility.Create<ImageSource?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (FavouriteControlButton)b;
                var value = (ImageSource?)v;
                This.ActivateImage.Source = value;
            });
    public ImageSource? ActivateImageSource {
        get => (ImageSource?)GetValue(ActivateImageSourceProperty);
        set => SetValue(ActivateImageSourceProperty, value);
    }
    public static readonly BindableProperty DeactivateImageSourceProperty
        = Utility.Create<ImageSource?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (FavouriteControlButton)b;
                var value = (ImageSource?)v;
                This.DeactivateImage.Source = value;
            });
    public ImageSource? DeactivateImageSource {
        get => (ImageSource?)GetValue(DeactivateImageSourceProperty);
        set => SetValue(DeactivateImageSourceProperty, value);
    }
    private bool _isFavourite = false;
    private readonly IFavouriteService _favouriteService;
    private readonly IQueueService _queueServce;
    private void RefreshDisplay() {
        this.ActivateImage.IsVisible = this._isFavourite;
        this.DeactivateImage.IsVisible = !this._isFavourite;
    }
    public FavouriteControlButton() {
        InitializeComponent();
        this._favouriteService = AppLifeCycle.Provider.GetRequiredService<IFavouriteService>();
        this._queueServce = AppLifeCycle.Provider.GetRequiredService<IQueueService>();

        // Lifetime event
        this._favouriteService.ItemsChanged += this.FavouriteService_ItemsChanged;
        this._queueServce.CurrentChanged += this.QueueServce_CurrentChanged;

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
        BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e) {
        BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
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