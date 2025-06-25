using MusicEco.ViewModels.Components;

namespace MusicEco.Views.Components;

public partial class InfoPreview : ContentView, IServiceAccess
{
    public InfoPreview()
	{
		InitializeComponent();
        this.BindingContext = IServiceAccess.Service.GetRequiredService<InfoPreviewModel>();

    }
}