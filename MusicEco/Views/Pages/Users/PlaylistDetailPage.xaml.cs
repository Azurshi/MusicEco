using MusicEco.ViewModels.Pages.Users;
using MusicEco.Views.Buttons;
using MusicEco.Views.Controls;
using MusicEco.Views.Items;

namespace MusicEco.Views.Pages.Users;

public partial class PlaylistDetailPage: ContentView {
    public PlaylistDetailPage(PlaylistDetailPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void MenuItemButton_Tapped(object sender, EventArgs e) {
        if (sender is MenuItemButton button) {
            var vm = (PlaylistDetailPageViewModel)this.BindingContext;
            vm.RemoveItemCommand.Execute(button.BindingContext);
        }
    }
}