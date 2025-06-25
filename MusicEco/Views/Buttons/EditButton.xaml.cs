
namespace MusicEco.Views.Buttons;

/// <summary>
/// Auto bind to BaseModfiableItem
/// </summary>
public partial class EditButton : BaseButton
{
	public EditButton()
	{
		InitializeComponent();
	}
	protected override Color HightLightColor => Colors.Green;
}