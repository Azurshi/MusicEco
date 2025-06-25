using MusicEco.ViewModels.Components;
namespace MusicEco.Views.Components;

public partial class QueueSelectionList : ContentView
{
	private readonly QueueSelectionListModel? ViewModel;
	public QueueSelectionList()
	{
		InitializeComponent();
	}
	public static async Task<QueueSelectionList> Create(long targetSongId, VoidEventHandler cleanup, bool isFile = false) {
        if (isFile) {
            targetSongId = QueueSelectionListModel.ConvertFromFileIdToSongId(targetSongId);
        }
        QueueSelectionListModel viewModel = new() {
            TargetSongId = targetSongId,
            CleaupFunction = cleanup
        };
        QueueSelectionList view = new();
        view.BindingContext = viewModel;
        await viewModel.LoadData();
        return view;
    }
}