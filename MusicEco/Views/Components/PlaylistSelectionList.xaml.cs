using MusicEco.ViewModels.Components;

namespace MusicEco.Views.Components;

public partial class PlaylistSelectionList : ContentView
{
	private readonly PlaylistSelectionListModel? ViewModel;
	public PlaylistSelectionList()
	{
		InitializeComponent();
	}
	public static async Task<PlaylistSelectionList> Create(long targetSongId, VoidEventHandler cleanup, bool isFile = false) {
        if (isFile) {
            targetSongId = PlaylistSelectionListModel.ConvertFromFileIdToSongId(targetSongId);
        }
        PlaylistSelectionListModel viewModel = new() {
            TargetSongId = targetSongId,
            CleaupFunction = cleanup
        };
        PlaylistSelectionList view = new();
		view.BindingContext = viewModel;
		await viewModel.LoadData();
		return view;
	}
}