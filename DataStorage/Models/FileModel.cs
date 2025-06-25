#if ANDROID
using AndroidX.DocumentFile.Provider;
#endif
using Domain.Models;
using System.Text.Json.Serialization;

namespace DataStorage.Models;
public class FileModel: ItemModel, IFileModel {
    internal static HashSet<long> missingIds = [];
    [JsonInclude] public long Size { get; set; } = DefaultValue.Id;
    [JsonInclude] public string Type { get; set; } = DefaultValue.Other;
    [JsonInclude] public string Sha256 { get; set; } = DefaultValue.Empty;
    [JsonInclude] public string Extension { get; set; } = DefaultValue.Empty;

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