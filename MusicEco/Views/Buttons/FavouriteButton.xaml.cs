
using Domain.EventSystem;
using Domain.Models;

namespace MusicEco.Views.Buttons;

public partial class FavouriteButton : BaseButton, IServiceAccess
{
	public FavouriteButton()
	{
		InitializeComponent();
		CommandParameterChanged += OnCommandParameterChanged;
		EventSystem.Connect<ViewModels.Components.ControlBarModel.FavouriteChangedEventArgs>(
			(s, e) => {
				OnCommandParameterChanged(null, EventArgs.Empty);
		});
	}
	protected override Color HightLightColor => Colors.LightPink;
	private void OnCommandParameterChanged(object? sender, EventArgs e) {
		object? param = CommandParameter;
		if (param != null) {
			long songId = long.Parse((string)param);
			ISongModel? songModel = IServiceAccess.ModelGetter.Song(songId);
			if (songModel != null) {
				if (songModel.Favourite) {
					this.Text = "Remove from favourite";
					return;
				}
			}
		}
		this.Text = "Add to favourite";
	}
}