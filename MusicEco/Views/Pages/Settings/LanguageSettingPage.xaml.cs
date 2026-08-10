using MusicEco.ViewModels.Pages.Settings;

namespace MusicEco.Views.Pages.Settings;

public partial class LanguageSettingPage: ContentView {
    public LanguageSettingPage(LanguageSettingPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}