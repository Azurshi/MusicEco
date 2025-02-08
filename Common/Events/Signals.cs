namespace MusicEco.Common.Events;
public enum Signal {
    Empty = 0,
    System_Before_UIStart,
    System_Data_Loaded,
    System_Data_Saved,
    System_Scanner_Finished,
    System_Extension_Loaded,

    Overlay_StartOptionMenu,
    Overlay_StopOptionMenu,
    Overlay_StartFormRequest,
    Overlay_StopFormRequest,
    Overlay_StartChangeAudio,
    Overlay_StopChangeAudio,

    UI_RowHeight_Changed,
    UI_WindowSize_Changed,
    UI_Section_Changed,
    UI_StartOverlay,
    UI_EngOverlay,
    Slot_Selected,
    Slot_Removed,
    Slot_Deleted,
    Slot_Favourited,
    Slot_Unfavourited,
    Slot_Added_ToPlaylist,
    Slot_Added_ToQueue,

    Player_PlayAudio_Requested,
    Player_Audio_Ended,
    Player_Song_Changed,
    Player_Progress_Changed,

    Song_Favourite_Changed,
    Song_Playcount_Changed,



    Dev_Button1,
    Dev_Button2, 
    Dev_Button3, 
    Dev_Button4, 
    Dev_Button5,
}
public enum MiniMenu {
    Empty = 0,
    AddToPlaylist,
    AddToQueue,
    Remove,
    Delete,
    Favourite,
    UnFavourite ,

    QueueList,
    PlaylistList
}