using MusicEco.Views.Widgets;

namespace MusicEco.Views.Sections.Album;

public partial class AlbumOverviewSlot : ContentView
{
    #region Base
    public static void OnLabelEntered(object sender, EventArgs e) => ViewUtil.OnEnteredSlot(sender, e);
    public static void OnLabelExited(object sender, EventArgs e) => ViewUtil.OnExitedSlot(sender, e);
    public static void OnEntered(object sender, EventArgs e) => ViewUtil.OnEntered(sender, e);
    public static void OnExited(object sender, EventArgs e) => ViewUtil.OnExited(sender, e);
    private ViewModels.Slots.AlbumOverviewSlot ViewModel => (ViewModels.Slots.AlbumOverviewSlot)this.BindingContext;
    public AlbumOverviewSlot() {
        InitializeComponent();
    }
    #endregion
    #region Slot
    private void Title_Clicked(object sender, TappedEventArgs e) {
        var slotData = ViewUtil.ChaseParent<ViewModels.Slots.AlbumOverviewSlot>(sender, out DataGrid dataGrid);
        dataGrid.SlotSelectedInvoke(slotData.Key);
    }
    #endregion
}