
using AndroidX.DocumentFile.Provider;
using Uri = Android.Net.Uri;
using Application = Android.App.Application;
using Android.Content;

namespace MusicEco.Platforms.Android;
public static class UriUtility {
    public static void Register(string path, Uri uri) {
        string? value = uri.ToString();
        if (value != null) {
            Preferences.Set($"saf_uri_{path}", value);
        }
    }
    public static Uri? GetUri(string path) {
        string? value = Preferences.Get($"saf_uri_{path}", null);
        return Uri.Parse(value);
    }
    #region Private
    private static string EncodeElement(string data) {
        return System.Uri.EscapeDataString(data);
    }
    private static string Decode(string data) {
        return System.Uri.UnescapeDataString(data);
    }
    #endregion
    public static string EncodeToUriPath(string filePath) {
        string[] segments = filePath.Split(":");
        for (int i = 1; i < segments.Length; i++) {
            segments[i] = EncodeElement(segments[i]);
        }
        return string.Join("%3A", segments);
    }
    public static string GetParentFolderPath(string itemPath) {
        int lastIndex = itemPath.LastIndexOf('/');
        if (lastIndex > 0) {
            itemPath = itemPath.Substring(0, lastIndex);
        }
        else {
            itemPath = "/";
        }
        return itemPath;
    }
    public static Uri? GetParentUri(Uri uri) {
        string? encodedPath = uri.EncodedPath ?? throw new NullReferenceException("Encoded path is null");
        string decodedPath = Decode(encodedPath);
        string decodedParent = GetParentFolderPath(decodedPath);
        string encodedParent = EncodeToUriPath(decodedParent);
        Uri.Builder? builder = uri.BuildUpon();
        builder?.EncodedPath(encodedParent);
        return builder?.Build();
    }
    public static List<string> GetUriChildrenNames(Uri uri) {
        List<string> nameList = [];
        DocumentFile directory = DocumentFile.FromTreeUri(Application.Context, uri) ?? throw new NullReferenceException();
        if (directory != null && directory.IsDirectory) {
            foreach (var file in directory.ListFiles()) {
                if (file.Name != null) {
                    nameList.Add(file.Name);
                }
            }
        }
        return nameList;
    }
    public static (List<Uri>, List<Uri>) GetUriChildren(Uri uri) {
        List<Uri> folderUris = [];
        List<Uri> fileUris = [];
        DocumentFile directory = DocumentFile.FromTreeUri(Application.Context, uri) ?? throw new NullReferenceException();
        if (directory != null && directory.IsDirectory) {
            foreach (var file in directory.ListFiles()) {
                if (file.IsDirectory) {
                    folderUris.Add(file.Uri);
                }
                else {
                    bool isValid = false;
                    foreach (var extension in Domain.Config.SupportedExtensions) {
                        if (file.Name != null && file.Name.Contains(extension)) {
                            isValid = true;
                            break;
                        }
                    }
                    if (isValid) {
                        fileUris.Add(file.Uri);
                    }
                }
            }
        }
        return (folderUris, fileUris);
    }
    public static string DirectFilePath(string filePath) {
        string[] parts = filePath.Split("primary:");
        string result = "/storage/emulated/0/" + parts[^1];
        return result;
    }
    public static async Task RequestSinglePermission<T>() where T : Permissions.BasePermission {
        var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted) {
            System.Diagnostics.Debug.WriteLine("Permission denied");
        }
        else {
            System.Diagnostics.Debug.WriteLine("Permission granted");
        }
    }

    public static async Task RequestPermission() {
        await RequestSinglePermission<Permissions.StorageWrite>();
    }
    public static string GetItemPathFromUri(Uri uri) {
        string folderPath = uri.EncodedPath ?? throw new NullReferenceException();
        string decodedPath = Decode(folderPath);
        string directPath = DirectFilePath(decodedPath);
        return directPath;
    }
    public static Uri? GetUriFomItemPath(string filePath) {
        var file = new Java.IO.File(filePath);
        var contentUri = Uri.FromFile(file);
        return contentUri;
    }
}
// Store as URI string