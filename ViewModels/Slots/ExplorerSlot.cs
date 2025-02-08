using MusicEco.Common.Value;
using MusicEco.Models;
using MusicEco.ViewModels.Base;
using System.Diagnostics;

namespace MusicEco.ViewModels.Slots;
public class ExplorerSlot : BaseSlot {
    private static readonly ColumnDefinitionCollection fileCols = [
    new () {Width = new (1, GridUnitType.Star)},
        new () {Width = new (1, GridUnitType.Absolute)},
        new () {Width = new (6, GridUnitType.Star)},
        new () {Width = new (1, GridUnitType.Absolute)},
        new () {Width = new (1, GridUnitType.Star)}
    ];
    private static readonly ColumnDefinitionCollection folderCols = [
        new () {Width = new (1, GridUnitType.Star)},
        new () {Width = new (1, GridUnitType.Absolute)},
        new () {Width = new (7, GridUnitType.Star)},
        new () {Width = new (1, GridUnitType.Absolute)} // Prevent last element unclickable 
    ];
    #region Databinding
    public string? Title { get; private set; }
    public ImageSource? Icon { get; private set; }
    private bool _isFile = false;
    public bool IsFile {
        get => _isFile;
        set {
            _isFile = value;
            if (value) {
                ColumnDefinitions = fileCols;
            } else {
                ColumnDefinitions = folderCols;
            }
        }
    }
    public ColumnDefinitionCollection ColumnDefinitions { get; private set; } = folderCols;
    #endregion
    protected override Task OnActive() {
        if (_key != null) {
            string[] compactKey = _key.Split("_");
            if (compactKey.Length == 2) {
                string itemType = compactKey[0];
                int id = int.Parse(compactKey[1]);
                if (itemType == Data.Item_FileType) {
                    FileModel? model = FileModel.Get(id);
                    if (model != null) {
                        Title = model.Name;
                        IsFile = true;
                        ImageModel? image = ImageModel.GetBySongFileId(id);
                        if (image != null) Icon = image.Icon;
                    }
                }
                else if (itemType == Data.Item_FolderType) {
                    FolderModel? model = FolderModel.Get(id);
                    if (model != null) {
                        Title = model.Name;
                        IsFile = false;
                        Icon = null;
                    }
                } else {
                    throw new ArgumentException("INVALID ITEM TYPE");
                }
            }
        }
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Icon));
        OnPropertyChanged(nameof(IsFile));
        OnPropertyChanged(nameof(ColumnDefinitions));
        return Task.CompletedTask;
    }
}