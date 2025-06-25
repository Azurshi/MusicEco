using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class OverviewPage : BasePage, IQueryAttributable
{
	public OverviewPage(OverviewPageModel viewModel)
	{
		InitializeComponent();
		MainBindingContext = viewModel;
	}
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        ((OverviewPageModel)this.MainBindingContext).ApplyQueryAttributes(query);
    }
}