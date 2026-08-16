using MusicEco.ViewModels.Pages.Settings;

namespace MusicEco.Views.Pages.Settings;

public partial class InterfaceSettingPage: ContentView {
    public InterfaceSettingPage(InterfaceSettingPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}