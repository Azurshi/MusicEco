using MusicEco.ViewModels.Components;

namespace MusicEco.Views.Components;

public partial class NavigationBar : ContentView, IServiceAccess {
    public NavigationBar() {
        InitializeComponent();
        this.BindingContext = IServiceAccess.Service.GetRequiredService<NavigationBarModel>();
    }
}