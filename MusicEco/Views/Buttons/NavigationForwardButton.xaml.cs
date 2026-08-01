using MusicEco.Core.Types;
using MusicEco.Services;

namespace MusicEco.Views.Buttons;

public partial class NavigationForwardButton: Button {
    public NavigationForwardButton() {
        InitializeComponent();
        var stack = AppLifeCycle.Provider.GetRequiredService<NavigationStack>();
        var command = new SyncCommandExtend(stack.NextPage, stack.CanNavigateToNextPage);
        stack.RouteChanged += (_, _) => command.NotifyCanExecute();
        this.Command = command;
    }
}