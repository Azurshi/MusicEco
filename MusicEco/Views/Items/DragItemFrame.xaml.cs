using MusicEco.SourceGeneration;
using MusicEco.Views.Buttons;
using System.Windows.Input;

namespace MusicEco.Views.Items;

public partial class DragItemFrame: ItemFrame {
    private static readonly Type ThisType = typeof(DragItemFrame);
    [BindableAutoGen]
    public static readonly BindableProperty DragStartCommandProperty
        = Utility.Create<ICommand>(ThisType, null);
    [BindableAutoGen] 
    public static readonly BindableProperty DragOverCommandProperty
        = Utility.Create<ICommand?>(ThisType, null);
    [BindableAutoGen]
    public static readonly BindableProperty DropCommandProperty
        = Utility.Create<ICommand?>(ThisType, null);
    public DragItemFrame() {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged() {
        base.OnBindingContextChanged();
        var value = this.BindingContext;
        this.DragGR.DragStartingCommandParameter = value;
        this.DropGR.DragOverCommandParameter = value;
        this.DropGR.DropCommandParameter = value;
    }

    private async void DragGR_DragStarting(object sender, DragStartingEventArgs e) {
        var command = this.DragGR.DragStartingCommand;
        if (command == null || !command.CanExecute(this.BindingContext)) {
            e.Cancel = true;
        }
        else {
            await Task.Delay(10);
            this.Container.IsVisible = false;
        }
    }

    private void DropGR_Drop(object sender, DropEventArgs e) {
        this.Container.BackgroundColor = Colors.Transparent;
    }

    private void DropGR_DragOver(object sender, DragEventArgs e) {
        this.Container.BackgroundColor = DynamicColors.ButtonHighlightColor;
    }

    private void DropGR_DragLeave(object sender, DragEventArgs e) {
        this.Container.BackgroundColor = Colors.Transparent;
    }
    public void Reset() {
        this.Container.IsVisible = true;
    }
}