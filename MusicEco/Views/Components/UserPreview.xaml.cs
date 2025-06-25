using MusicEco.ViewModels.Components;

namespace MusicEco.Views.Components;
public partial class UserPreview : ContentView, IServiceAccess
{
    public UserPreview()
	{
		InitializeComponent();
		this.BindingContext = IServiceAccess.Service.GetRequiredService<UserPreviewModel>();
	}
}