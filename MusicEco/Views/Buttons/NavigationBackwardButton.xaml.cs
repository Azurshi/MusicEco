using MusicEco.Core.Types;
using MusicEco.Services;

namespace MusicEco.Views.Buttons;

public partial class NavigationBackwardButton: Button {
    public NavigationBackwardButton() {
        InitializeComponent();
        var stack = AppLifeCycle.Provider.GetRequiredService<NavigationStack>();
        var command = new SyncCommandExtend(stack.PreviousPage, stack.CanNavigateToPreviousPage);
        stack.RouteChanged += (_, _) => command.NotifyCanExecute();
        this.Command = command;
    }
}