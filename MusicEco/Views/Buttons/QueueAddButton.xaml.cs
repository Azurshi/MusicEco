
using MusicEco.Views.Components;
using MusicEco.Views.PageExtensions;

namespace MusicEco.Views.Buttons;

public partial class QueueAddButton : BaseButton
{
    public static readonly BindableProperty IsFileProperty =
    Utility.Create<bool>(typeof(QueueAddButton));
    public bool IsFile {
        get => (bool)GetValue(IsFileProperty);
        set => SetValue(IsFileProperty, value);
    }
    public QueueAddButton()
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
            QueueSelectionList view = await QueueSelectionList.Create(songId, page.PageOverlay.Stop, IsFile);
            page.PageOverlay.StartAuto(view);
        }
    }
}