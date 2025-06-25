namespace MusicEco.Views.Widgets;

public partial class ItemFrame : Border
{
    public ItemFrame()
	{
		InitializeComponent();
        PreviousColor = this.BackgroundColor;
    }
    private static readonly Color HoverColor = (Color)Application.Current!.Resources["ItemHoverColor"];
    private Color PreviousColor;
    private void PointerEntered(object sender, PointerEventArgs e) {
        PreviousColor = this.BackgroundColor;
        BackgroundColor = HoverColor;
    }
    private void PointerExited(object sender, PointerEventArgs e) {
        BackgroundColor = PreviousColor;
    }
}