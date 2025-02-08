using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Views.Sections.Common;
using MusicEco.Views.Widgets;

namespace MusicEco.Views.Sections.Search;

public partial class SearchSlot : ContentView
{
    #region Base
    public static void OnLabelEntered(object sender, EventArgs e) => ViewUtil.OnEnteredSlot(sender, e);
    public static void OnLabelExited(object sender, EventArgs e) => ViewUtil.OnExitedSlot(sender, e);
    public static void OnEntered(object sender, EventArgs e) => ViewUtil.OnEntered(sender, e);
    public static void OnExited(object sender, EventArgs e) => ViewUtil.OnExited(sender, e);
    private ViewModels.Slots.SongSlot ViewModel => (ViewModels.Slots.SongSlot)this.BindingContext;
    public SearchSlot()
	{
		InitializeComponent();
	}
    private const string SlotKind = "Search";
    #endregion
    #region Slot
    private void Title_Clicked(object sender,  TappedEventArgs e) {
        var slotData = ViewUtil.ChaseParent<ViewModels.Slots.SongSlot>(sender, out DataList dataList);
        dataList.SlotSelectedInvoke(slotData.Key);
    }
    private void Option_Clicked(object sender, TappedEventArgs e) {
        var slotData = ViewUtil.ChaseParent<ViewModels.Slots.SongSlot>(sender, out DataList dataList);
        dataList.SlotOptionInvoke(slotData.Key);
        DataTemplate template = (DataTemplate)this.Resources[$"{SlotKind}OptionTemplate"];
        VisualElement menu = (VisualElement)template.CreateContent();
        EventSystem.Publish<OptionMenuEventArgs>(Signal.Overlay_StartOptionMenu, this, new(menu, e));
    }
    #endregion
    #region OptionMenu
    private void AddToQueueOption_Clicked(object sender, TappedEventArgs e) {
        EventSystem.Publish(Signal.Overlay_StopOptionMenu, this);
        DataTemplate template = (DataTemplate)this.Resources[$"{SlotKind}QueueSelectTemplate"];
        PlaylistList playlistList = (PlaylistList)template.CreateContent();
        playlistList.SetType(true);
        EventSystem.Publish<FormMenuEventArgs>(Signal.Overlay_StartFormRequest, this, new(playlistList));
    }
    private void AddToPlaylistOption_Clicked(object sender, TappedEventArgs e) {
        EventSystem.Publish(Signal.Overlay_StopOptionMenu, this);
        DataTemplate template = (DataTemplate)this.Resources[$"{SlotKind}PlaylistSelectTemplate"];
        PlaylistList playlistList = (PlaylistList)template.CreateContent();
        playlistList.SetType(false);
        EventSystem.Publish<FormMenuEventArgs>(Signal.Overlay_StartFormRequest, this, new(playlistList));
    }
    #endregion
    #region 
    private void PlaylistList_SlotSelected(object sender, StringEventArgs e) {
        ViewModel.AddToPlaylist(int.Parse(e.Value));
        EventSystem.Publish(Signal.Overlay_StopFormRequest, this);
    }
    private void QueueList_SlotSelected(object sender, StringEventArgs e) {
        ViewModel.AddToQueue(int.Parse(e.Value));
        EventSystem.Publish(Signal.Overlay_StopFormRequest, this);
    }
    #endregion
}