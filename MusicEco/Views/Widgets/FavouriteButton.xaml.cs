namespace MusicEco.Views.Widgets;

public partial class FavouriteButton : ImageButton
{
    #region Binding
    public static readonly BindableProperty IsFavouriteProperty =
        Utility.Create<bool>(typeof(FavouriteButton), bindingMode:BindingMode.TwoWay, propertyChanged:(s, _, v) => {
            FavouriteButton This = (FavouriteButton)s;
            bool value = (bool)v;
            if (value) {
                This.Source = FavouriteImageSource;
            }
            else {
                This.Source = UnFavouriteImageSource;
            }
        });
    public bool IsFavourite {
        get => (bool)GetValue(IsFavouriteProperty);
        set => SetValue(IsFavouriteProperty, value);
    }
    #endregion
    private static readonly ImageSource FavouriteImageSource = ImageSource.FromFile("favourite.png");
    private static readonly ImageSource UnFavouriteImageSource = ImageSource.FromFile("no_favourite.png");
    public FavouriteButton()
	{
		InitializeComponent();
        IsFavourite = false;
        Source = UnFavouriteImageSource;
	}
    protected override void OnClicked(object sender, EventArgs e) {
        IsFavourite = !IsFavourite;
        InvokeClicked(e);
        command?.Execute(commandParameter);
    }
}