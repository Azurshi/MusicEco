using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Models;
using MusicEco.ViewModels.Base;
using MusicEco.ViewModels.Slots;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels.Sections;
public class SearchSection : PropertyObject {
    public SearchSection() {
        DataController = new([], Setting.ListFrameAmount);
        EventSystem.Connect(Signal.System_Data_Loaded, OnDataLoaded);
    }
    private async void OnDataLoaded(object? sender, EventArgs e) {
        await UpdateData();
    }
    public readonly ObservableCollectionController<SongSlot> DataController;
    public ObservableCollection<BaseSlot> Data => DataController.Target;
    public string LastSearchedName = "Search";
    #region DataModify
    public async Task UpdateData() {
        List<SongModel> models = SongModel.SearchByName(LastSearchedName);
        List<string> ids = [];
        foreach (var model in models) {
            ids.Add(model.Id.ToString());
        }
        await DataController.UpdateKeysAsync(ids);
    }
    public void Search(string searchName) {
        LastSearchedName = searchName;
    }
    #endregion
}