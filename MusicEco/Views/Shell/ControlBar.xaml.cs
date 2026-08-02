using MusicEco.ViewModels.Shell;

namespace MusicEco.Views.Shell;

public partial class ControlBar: ContentView {
    public ControlBar(ControlBarViewModel viewModel) {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
    private readonly double _itemScale = 1.5;
    private readonly double _volumeScale = 3;
    private void ButtonContainer_SizeChanged(object? sender, EventArgs e) {
        int childCount = ButtonContainer.Children.Count;
        double itemWidth = ButtonContainer.Height * _itemScale;
        double totalSpacing = ButtonContainer.Width - itemWidth * childCount;
        totalSpacing = Math.Max(0, totalSpacing);
        double spacing = 0;
        if (childCount - 1 > 0) {
            spacing = totalSpacing / (childCount - 1);
        }
        ButtonContainer.ColumnSpacing = spacing;
        double volumeSliderWidth = ButtonContainer.Height * _volumeScale;
        double volumeSliderLeftMargin = ButtonContainer.Height * (_volumeScale - _itemScale) / 2;
        VolumeSlider.WidthRequest = volumeSliderWidth;
        Overlay.Margin = new(itemWidth + spacing - volumeSliderLeftMargin, 14, 0, 0);
    }
}