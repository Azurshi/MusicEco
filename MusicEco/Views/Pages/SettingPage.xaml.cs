using MusicEco.ViewModels.Pages;

namespace MusicEco.Views.Pages;

public partial class SettingPage: ContentView {
    public SettingPage(SettingPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}