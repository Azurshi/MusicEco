using MusicEco.ViewModels.Items;
using MusicEco.ViewModels.Pages;
using MusicEco.Views.Buttons;
using MusicEco.Views.Items;
using System.Diagnostics;

namespace MusicEco.Views.Pages;

public partial class QueueDetailPage: ContentView {
    public QueueDetailPage(QueueDetailPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void MenuItemButton_Tapped(object sender, EventArgs e) {
        if (sender is MenuItemButton button) {
            var vm = (QueueDetailPageViewModel)this.BindingContext;
            vm.RemoveItemCommand.Execute(button.BindingContext);
        }
    }
    private void DropGestureRecognizer_Drop(object sender, DropEventArgs e) {
        if (sender is CollectionView collection) {
            var items = collection.GetVisualTreeDescendants().OfType<DragItemFrame>();
            foreach (var item in items) {
                item.Reset();
            }
        }
    }
}