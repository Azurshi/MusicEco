using MusicEco.ViewModels.Pages.Settings;

namespace MusicEco.Views.Pages.Settings;

public partial class BackupSettingPage: ContentView {
    public BackupSettingPage(BackupSetttingPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}