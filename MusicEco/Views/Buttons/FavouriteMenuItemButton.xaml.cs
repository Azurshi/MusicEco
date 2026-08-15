using MusicEco.Core.Services;
using MusicEco.Services;
using MusicEco.Views.Commands;
using MusicEco.Views.Items;

namespace MusicEco.Views.Buttons;

public partial class FavouriteMenuItemButton: ContentView, IMenuItemButton {
    private readonly AssemblyLocalization L;
    private readonly IFavouriteService _favouriteService;
    private bool? _isFavourite = null;
    public event EventHandler? Tapped;
    public FavouriteMenuItemButton() {
        InitializeComponent();
        this.L = AppLifeCycle.Provider.GetRequiredService<ILocalizationService>().Get(this.GetType());
        this._favouriteService = AppLifeCycle.Provider.GetRequiredService<IFavouriteService>();
        this.BindingContextChanged += this.OnBindingContextChanged;
    }

    private async void OnBindingContextChanged(object? sender, EventArgs e) {
        if (MenuCommands.TryGetHash(this.BindingContext, out var hash)) {
            if (await this._favouriteService.IsFavourite(hash)) {
                this._isFavourite = true;
                this.InnerLabel.Text = L["Menu_RemoveFromFavourite"];
            }
            else {
                this._isFavourite = false;
                this.InnerLabel.Text = L["Menu_AddToFavourite"];
            }
        }
    }

    private async void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e) {
        Tapped?.Invoke(this, EventArgs.Empty);
        if (this._isFavourite != null) {
            if (MenuCommands.TryGetHash(this.BindingContext, out var hash)) {
                if (this._isFavourite.Value) {
                    await this._favouriteService.RemoveFavourite(hash, this);
                }
                else {
                    await this._favouriteService.AddFavourite(hash, this);
                }
            }
        }
    }

    private void PointerGestureRecognizer_PointerEntered(object? sender, PointerEventArgs e) {
        this.BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void PointerGestureRecognizer_PointerExited(object? sender, PointerEventArgs e) {
        this.BackgroundColor = Colors.Transparent;
    }
}