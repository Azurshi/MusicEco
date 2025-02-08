using MusicEco.Common.Events;
using MusicEco.Views.Widgets;

namespace MusicEco.Views.Sections.Common;

public partial class PlaylistList : ContentView {

    public static void OnLabelEntered(object sender, EventArgs e) => ViewUtil.OnEnteredSlot(sender, e);
    public static void OnLabelExited(object sender, EventArgs e) => ViewUtil.OnExitedSlot(sender, e);
    public static void OnEntered(object sender, EventArgs e) => ViewUtil.OnEntered(sender, e);
    public static void OnExited(object sender, EventArgs e) => ViewUtil.OnExited(sender, e);
    public event EventHandler<StringEventArgs>? SlotSelected;
    private readonly ViewModels.Misc.PlaylistList _viewModel;
    public PlaylistList() {
        InitializeComponent();

        _viewModel = (ViewModels.Misc.PlaylistList)this.BindingContext;
    }
    private void OnLabelClicked(object sender, EventArgs e) {
        var slotData = ViewUtil.ChaseParent<ViewModels.Slots.PlaylistSlot>(sender, out DataList dataList);
        dataList.SlotSelectedInvoke(slotData.Key);
        SlotSelected?.Invoke(sender, new(slotData.Key));
    }

    private async void ContentList_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.DataController.PageDown(e.Value);
    }

    public void SetType(bool isQueue = true) {
        _viewModel.SetQueryType(isQueue);
    }
}