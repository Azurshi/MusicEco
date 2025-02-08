using MusicEco.Views.Widgets;

namespace MusicEco.Views;
public static class ViewUtil {
    public static T ChaseParent<T>(object sender, out DataList dataList) where T : ViewModels.Base.BaseSlot {
        VisualElement visualElement = (VisualElement)sender;
        ViewModels.Base.BaseSlot slotData = (ViewModels.Base.BaseSlot)visualElement.BindingContext;
        while (visualElement is not DataList) {
            visualElement = (VisualElement)visualElement.Parent;
        }
        dataList = (DataList)visualElement;
        return (T)slotData;
    }
    public static T ChaseParent<T>(object sender, out DataGrid dataGrid) where T : ViewModels.Base.BaseSlot {
        VisualElement visualElement = (VisualElement)sender;
        ViewModels.Base.BaseSlot slotData = (ViewModels.Base.BaseSlot)visualElement.BindingContext;
        while (visualElement is not DataGrid) {
            visualElement = (VisualElement)visualElement.Parent;
        }
        dataGrid = (DataGrid)visualElement;
        return (T)slotData;
    }
    private static readonly Color SlotColor = (Color)Application.Current!.Resources["SlotBackground"];
    public static void OnEnteredSlot(object sender, EventArgs e) {
        if (sender is LabelButton labelButton && labelButton.Parent is Grid grid) grid.BackgroundColor = Colors.LightBlue;
    }
    public static void OnExitedSlot(object sender, EventArgs e) {
        if (sender is LabelButton labelButton && labelButton.Parent is Grid grid) grid.BackgroundColor = SlotColor;
    }
    private static readonly Color ElementColor = (Color)Application.Current!.Resources["BCBlack"];
    public static void OnEntered(object sender, EventArgs e) {
        ((VisualElement)sender).BackgroundColor = Colors.LightBlue;
    }
    public static void OnExited(object sender, EventArgs e) {
        ((VisualElement)sender).BackgroundColor = ElementColor;
    }
}