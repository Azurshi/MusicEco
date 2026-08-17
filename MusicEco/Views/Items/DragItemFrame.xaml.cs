using System.Windows.Input;

namespace MusicEco.Views.Items;

public partial class DragItemFrame: ItemFrame {
    private static readonly Type ThisType = typeof(DragItemFrame);
    public static readonly BindableProperty DragStartCommandProperty
        = Utility.Create<ICommand?>(ThisType, null,
            propertyChanged: (b, _, v) => {
                var This = (DragItemFrame)b;
                var value = (ICommand?)v;
                This.DragGR.DragStartingCommand = value;
            });
    public ICommand? DragStartCommand {
        get => (ICommand?)GetValue(DragStartCommandProperty);
        set => SetValue(DragStartCommandProperty, value);
    }
    public static readonly BindableProperty DragOverCommandProperty
        = Utility.Create<ICommand?>(ThisType, null,
            propertyChanged: (b, _, v) => {
                var This = (DragItemFrame)b;
                var value = (ICommand?)v;
                This.DropGR.DragOverCommand = value;
            });
    public ICommand? DragOverCommand {
        get => (ICommand?)GetValue(DragOverCommandProperty);
        set => SetValue(DragOverCommandProperty, value);
    }
    public static readonly BindableProperty DropCommandProperty
        = Utility.Create<ICommand?>(ThisType, null,
            propertyChanged: (b, _, v) => {
                var This = (DragItemFrame)b;
                var value = (ICommand?)v;
                This.DropGR.DropCommand = value;
            });
    public ICommand? DropCommand {
        get => (ICommand?)GetValue(DropCommandProperty);
        set => SetValue(DropCommandProperty, value);
    }
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
        this.Container.BackgroundColor = Utility.GetResource<Color>("ButtonHighlightColor");
    }

    private void DropGR_DragLeave(object sender, DragEventArgs e) {
        this.Container.BackgroundColor = Colors.Transparent;
    }
    public void Reset() {
        this.Container.IsVisible = true;
    }
}