#if ANDROID
using AndroidX.DocumentFile.Provider;
#endif
using Domain.Models;
using System.Text.Json.Serialization;

namespace DataStorage.Models;
public class FolderModel : ItemModel, IFolderModel {
    internal static HashSet<long> missingIds = [];
    [JsonIgnore] public List<IFolderModel> ChildFolders {
        get => GetAll<FolderModel>().Where(s => s.ParentId == this.Id).Select(s => (IFolderModel)s).ToList();
    }
    [JsonIgnore] public List<IFileModel> ChildFiles {
        get => GetAll<FileModel>().Where(s => s.ParentId == this.Id).Select(s => (IFileModel)s).ToList();
    }
    public override void Refresh() {
        if (missingIds.Contains(this.id)) {
            availble = false;
            return;
        }
#if WINDOWS
        if (Source == ItemSource.Windows) {
            if (CustomFile.Exists(Path)) {
                availble = true;
            }
            else {
                availble = false;
            }
        }
#endif
#if ANDROID
        if (Source == ItemSource.Androids) {
            Android.Net.Uri uri = Android.Net.Uri.Parse(Path)!;
            DocumentFile? docFile = DocumentFile.FromSingleUri(Android.App.Application.Context, uri);
            availble = docFile != null && docFile.Exists();
        }
#endif
        else if (Source == ItemSource.Unknown || Source == 0) {
            throw new Exception("Fatal error");
        }
    }
}