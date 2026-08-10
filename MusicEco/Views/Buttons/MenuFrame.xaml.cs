using MusicEco.Views.Overlays;

namespace MusicEco.Views.Buttons;

public partial class MenuFrame: ContentView, IOverlay {
    public event EventHandler? Closed;
    public MenuFrame() {
        InitializeComponent();
    }
    public void ForceClose() {
        Closed?.Invoke(this, EventArgs.Empty);
    }
    public void Assign(Layout layout) {
        this.Container.Content = layout;
        Queue<Layout> q = [];
        q.Enqueue(layout);
        while(q.Count > 0) {
            layout = q.Dequeue();
            foreach (var item in layout.Children) {
                if (item is Layout childLayout) {
                    q.Enqueue(childLayout);
                }
                else if (item is Button button) {
                    button.Clicked += this.Button_Clicked;
                }
            }
        }
    }
    private void Button_Clicked(object? sender, EventArgs e) {
        Closed?.Invoke(this, EventArgs.Empty);
    }
}