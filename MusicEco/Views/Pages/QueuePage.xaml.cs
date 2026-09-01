using MusicEco.ViewModels;
using MusicEco.ViewModels.Pages;
using MusicEco.Views.Buttons;

namespace MusicEco.Views.Pages;

public partial class QueuePage: ContentView {
    public QueuePage(QueuePageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void Menu_RemoveButton_Tapped(object sender, EventArgs e) {
        if (sender is MenuItemButton button) {
            var vm = (QueuePageViewModel)this.BindingContext;
            vm.RemoveItemCommand.Execute(button.BindingContext);
        }
    }
}