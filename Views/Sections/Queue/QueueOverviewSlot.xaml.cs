using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Views.Widgets;
namespace MusicEco.Views.Sections.Queue;

public partial class QueueOverviewSlot : ContentView
{
    #region Base
    public static void OnLabelEntered(object sender, EventArgs e) => ViewUtil.OnEnteredSlot(sender, e);
    public static void OnLabelExited(object sender, EventArgs e) => ViewUtil.OnExitedSlot(sender, e);
    public static void OnEntered(object sender, EventArgs e) => ViewUtil.OnEntered(sender, e);
    public static void OnExited(object sender, EventArgs e) => ViewUtil.OnExited(sender, e);
    private ViewModels.Slots.PlaylistSlot ViewModel => (ViewModels.Slots.PlaylistSlot)this.BindingContext;
    public QueueOverviewSlot() {
        InitializeComponent();
    }
    private const string SlotKind = "QueueOverview";
    #endregion
    #region Slot
    public void Title_Clicked(object sender, TappedEventArgs e) {
        var slotData = ViewUtil.ChaseParent<ViewModels.Slots.PlaylistSlot>(sender, out DataList dataList);
        dataList.SlotSelectedInvoke(slotData.Key);
    }
    private void Option_Clicked(object sender, TappedEventArgs e) {
        var slotData = ViewUtil.ChaseParent<ViewModels.Slots.PlaylistSlot>(sender, out DataList dataList);
        dataList.SlotOptionInvoke(slotData.Key);
        DataTemplate template = (DataTemplate)this.Resources[$"{SlotKind}OptionTemplate"];
        VisualElement menu = (VisualElement)template.CreateContent();
        EventSystem.Publish<OptionMenuEventArgs>(Signal.Overlay_StartOptionMenu, this, new(menu, e));
    }
    #endregion
    #region OptionMenu
    private void RenameOption_Clicked(object sender, TappedEventArgs e) {
        DialogButton.ConfirmColor = Colors.Green;
        DialogButton.CancelColor = Colors.Red;
        TitleButton.IsVisible = false;
        OptionButton.IsVisible = false;
        TitleInput.Text = TitleButton.Text;
        TitleInput.IsVisible = true;
        DialogButton.IsVisible = true;
        EventSystem.Publish(Signal.Overlay_StopOptionMenu, this);
    }
    private void DeleteOption_Clicked(object sender, TappedEventArgs e) {
        DialogButton.ConfirmColor = Colors.Red;
        DialogButton.CancelColor = Colors.Green;
        Grid.SetColumn(DialogButton, 0);
        Grid.SetColumnSpan(DialogButton, 3);
        TitleButton.IsVisible = false;
        OptionButton.IsVisible = false;
        DialogButton.IsVisible = true;
        EventSystem.Publish(Signal.Overlay_StopOptionMenu, this);
    }
    private void DialogButton_ConfirmClicked(object sender, EventArgs e) {
        if (TitleInput.IsVisible) {
            TitleInput.IsVisible = false;
            DialogButton.IsVisible = false;
            TitleButton.IsVisible = true;
            OptionButton.IsVisible = true;
            ViewModel.ChangeName(TitleInput.Text);
        }
        else {
            Grid.SetColumn(DialogButton, 2);
            Grid.SetColumnSpan(DialogButton, 1);
            DialogButton.IsVisible = false;
            TitleButton.IsVisible = true;
            OptionButton.IsVisible = true;
            var slotData = ViewUtil.ChaseParent<ViewModels.Slots.PlaylistSlot>(sender, out DataList dataList);
            dataList.ItemsSource.Remove(slotData);
            ViewModel.DeleteSlot();
        }
    }
    private void DialogButton_CancelClicked(object sender, EventArgs e) {
        if (TitleInput.IsVisible) {
            TitleInput.IsVisible = false;
            DialogButton.IsVisible = false;
            TitleButton.IsVisible = true;
            OptionButton.IsVisible = true;
        }
        else {
            Grid.SetColumn(DialogButton, 2);
            Grid.SetColumnSpan(DialogButton, 1);
            DialogButton.IsVisible = false;
            TitleButton.IsVisible = true;
            OptionButton.IsVisible = true;
        }
    }
    #endregion OptionMenu
}