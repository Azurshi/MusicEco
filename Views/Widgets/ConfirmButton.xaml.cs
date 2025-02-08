namespace MusicEco.Views.Widgets;

public partial class ConfirmButton : ContentView
{
	public event EventHandler? ConfirmClicked;
	public event EventHandler? CancelClicked;
    public Color ConfirmColor = Colors.Green;
    public Color CancelColor = Colors.Red;
    public Color ElementColor = (Color)Application.Current!.Resources["SlotBackground"];

    public LabelButton ConfirmElement => this.ConfirmInner;
    public LabelButton CancelElement => this.CancelInner;
	public ConfirmButton()
	{
		InitializeComponent();
	}

    private void ConfirmButton_Clicked(object sender, TappedEventArgs e) {
        ConfirmClicked?.Invoke(this, e);
    }
    private void CancelButton_Clicked(object sender, TappedEventArgs e) {
        CancelClicked?.Invoke(this, e);
    }
    private void ConfirmButton_Entered(object sender, EventArgs e) {
        ConfirmInner.BackgroundColor = ConfirmColor;
    }

    private void ConfirmButton_Exited(object sender, EventArgs e) {
        ConfirmInner.BackgroundColor = ElementColor;
    }

    private void CancelButton_Entered(object sender, EventArgs e) {
        CancelInner.BackgroundColor = CancelColor;
    }

    private void CancelButton_Exited(object sender, EventArgs e) {
        CancelInner.BackgroundColor = ElementColor;

    }
}