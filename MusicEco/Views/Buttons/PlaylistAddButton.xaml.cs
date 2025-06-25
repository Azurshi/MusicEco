using MusicEco.Views.Components;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.Buttons;

public partial class PlaylistAddButton : BaseButton
{
    public static readonly BindableProperty IsFileProperty =
        Utility.Create<bool>(typeof(PlaylistAddButton));
    public bool IsFile {
        get => (bool)GetValue(IsFileProperty);
        set => SetValue(IsFileProperty, value);
    }
	public PlaylistAddButton()
	{
		InitializeComponent();
	}
    protected override async void OnClicked(object sender, TappedEventArgs e) {
        command?.Execute(commandParameter);
        GetBasePageEventArgs arg = new();
        InvokeGetPage(arg);
        IBasePage? page = arg.Page;
        if (page != null && commandParameter != null) {
            long songId = long.Parse((string)commandParameter);
            page.PageOverlay.Stop();
            PlaylistSelectionList view = await PlaylistSelectionList.Create(songId, page.PageOverlay.Stop, IsFile);
            page.PageOverlay.StartAuto(view);
        }
    }
}