using MusicEco.ViewModels.Items;
using MusicEco.ViewModels.Pages;
using System.Diagnostics;

namespace MusicEco.Views.Pages;

public partial class QueueDetailPage: ContentView {
    public QueueDetailPage(QueueDetailPageViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void Button_Clicked(object sender, EventArgs e) {
        if (sender is View view) {
            Debug.WriteLine(view.BindingContext);
            if (view is Button button) {
                Debug.WriteLine(button.CommandParameter);
            }
            if (view.BindingContext is AudioEntryViewModel vm) {
                Debug.WriteLine($"{vm.FileHash} : {vm.DisplayTitle}");
            }
        }
    }
}