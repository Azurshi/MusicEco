using MusicEco.Common.Events;
using MusicEco.Common.Value;
using MusicEco.Models;
using System.Diagnostics;


namespace MusicEco.Views.Sections;

public partial class ExplorerSection : ContentView
{
    private readonly ViewModels.Sections.ExplorerSection _viewModel;
	public ExplorerSection()
	{
		InitializeComponent();
        _viewModel = (ViewModels.Sections.ExplorerSection)this.BindingContext;
        _viewModel.UpdateData().GetAwaiter().GetResult();
	}
    #region Signal
    private async void ExplorerList_SlotSelected(object sender, StringEventArgs e) {
        string[] compactKey = e.Value.Split("_");
        if (compactKey.Length == 2) {
            string itemType = compactKey[0];
            int id = int.Parse(compactKey[1]);
            if (itemType == Data.Item_FolderType) {
                ViewModels.Sections.ExplorerSection.Select(id);
                _viewModel.SelectFolder(id);
                await _viewModel.UpdateData();
            }
            else if (itemType == Data.Item_FileType) {
                Debug.WriteLine("Explorer endpoint " + e.Value);
                List<int> songIds = _viewModel.ExtractSongIds();
                ViewCenter.AddOrUpdateQueue(
                    $"Folder {_viewModel.LastSelectedFolderName}",
                    SongModel.GetByFileId(id)?.Id ?? -1,
                    songIds);
            }
            else {
                throw new ArgumentException("INVALID ITEM TYPE");
            }
        }
    }
    private async void OnBackwardClicked(object sender, EventArgs e) {
        ViewModels.Sections.ExplorerSection.Backward();
        int currentId = ViewModels.Sections.ExplorerSection.CurrentId;
        _viewModel.SelectFolder(currentId);
        await _viewModel.UpdateData();
    }
    private async void OnForwardClicked(object sender, EventArgs e) {
        ViewModels.Sections.ExplorerSection.Forward();
        int currentId = ViewModels.Sections.ExplorerSection.CurrentId;
        _viewModel.SelectFolder(currentId);
        await _viewModel.UpdateData();
    }
    private async void OnUpClicked(object sender, EventArgs e) {
        ViewModels.Sections.ExplorerSection.Up();
        int currentId = ViewModels.Sections.ExplorerSection.CurrentId;
        _viewModel.SelectFolder(currentId);
        await _viewModel.UpdateData();
    }
    private async void OnRefreshClicked(object sender, EventArgs e) {
        int currentId = ViewModels.Sections.ExplorerSection.CurrentId;
        _viewModel.SelectFolder(currentId);
        await _viewModel.UpdateData();
    }
    private async void ExplorerList_LoadMoreItemRequest(object sender, IntEventArgs e) {
        await _viewModel.DataController.PageDown(e.Value);
    }
    #endregion
}