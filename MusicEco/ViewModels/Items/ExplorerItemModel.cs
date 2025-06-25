using Domain.Models;
using System.Diagnostics;

namespace MusicEco.ViewModels.Items;
public partial class ExplorerItemModel : BaseItem, IServiceAccess {
    public string? Title { get; set; }
    public ImageSource? Icon { get; set; }
    private static readonly List<string> _propertyNames = [
        nameof(Key), nameof(IsFile), nameof(IsFolder), nameof(Title), nameof(Icon), nameof(Available)
        ];
    public bool IsFile { get; set; }
    public bool IsFolder { get; set; }
    public bool Available { get; set; }
    protected override async Task OnActive() {
        if (Key == string.Empty) return;
        IFolderModel? folderModel = IServiceAccess.ModelGetter.Folder(long.Parse(Key));
        if (folderModel != null) {
            IsFile = false;
            IsFolder = true;
            Title = folderModel.Name;
            Available = folderModel.Available;
            Debug.WriteLine(Available);
            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
        }
        else {
            IFileModel? fileModel = IServiceAccess.ModelGetter.File(long.Parse(Key));
            if (fileModel != null) {
                IsFile = true;
                IsFolder = false;
                Title = fileModel.Name;
                Available = fileModel.Available;
                Icon = IServiceAccess.DataGetter.Icon(fileModel);
                foreach (var propertyName in _propertyNames) {
                    OnPropertyChanged(propertyName);
                }
            }
        }
        await Task.CompletedTask;
    }
}