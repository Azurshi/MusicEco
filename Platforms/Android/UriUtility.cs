using Android.Content;
using Android.Provider;
using AndroidX.DocumentFile.Provider;

using Uri = Android.Net.Uri;
using Application = Android.App.Application;
using Enviroment = Android.OS.Environment;
using MusicEco.Common;
using Android.App;
using Bumptech.Glide.Load.Engine;

namespace MusicEco.Platforms.Android;
public static class UriUtility {
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
                    foreach (var extension in Setting.SupportedExtension) {
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

//#pragma warning disable CS8600, CS8604, CS8603, CS8602
//[Obsolete]
//public static class UriUtil {
//    public static string Encode(string data) {
//        string[] segments = data.Split(":");
//        for (int i = 1; i < segments.Length; i++) {
//            segments[i] = EncodeElement(segments[i]);
//        }
//        return string.Join("%3A", segments);
//    }
//    public static string EncodeElement(string data) {
//        return System.Uri.EscapeDataString(data);
//    }
//    public static string Decode(string data) {
//        return System.Uri.UnescapeDataString(data);
//    }
//    public static string Parent(string decodedPath) {
//        int lastIndex = decodedPath.LastIndexOf('/');
//        if (lastIndex > 0) {
//            decodedPath = decodedPath.Substring(0, lastIndex);
//        }
//        else {
//            decodedPath = "/";
//        }
//        return decodedPath;
//    }
//    public static Uri GetParent(Uri uri) {
//        string encodedPath = uri.EncodedPath;
//        string decodedPath = Decode(encodedPath);
//        string decodedParent = Parent(decodedPath);
//        string encodedParent = Encode(decodedParent);
//        Uri.Builder builder = uri.BuildUpon();
//        builder.EncodedPath(encodedParent);
//        return builder.Build();
//    }
//    public static Uri AppendChild(Uri uri, string childName) {
//        //string encodedPath = uri.EncodedPath;
//        //string encodedChild = encodedPath + "%2F" + EncodeElement(childName);
//        Uri.Builder builder = uri.BuildUpon();
//        builder.AppendPath(EncodeElement(childName));
//        return builder.Build();

//    }
//    public static List<string> ChildrenName(Uri uri) {
//        List<string> itemList = new List<string>();
//        ContentResolver resolver = Application.Context.ContentResolver;
//        DocumentFile directory = DocumentFile.FromTreeUri(Application.Context, uri);
//        if (directory != null && directory.IsDirectory) {
//            foreach (var file in directory.ListFiles()) {
//                itemList.Add(file.Name);
//                System.Diagnostics.Debug.WriteLine("F : " + file.Name);
//            }
//        }
//        return itemList;
//    }
//    public static (List<Uri>, List<Uri>) Children(Uri uri) {
//        List<Uri> folderUris = new List<Uri>();
//        List<Uri> fileUris = new List<Uri>();
//        ContentResolver resolver = Application.Context.ContentResolver;
//        DocumentFile directory = DocumentFile.FromTreeUri(Application.Context, uri);
//        if (directory != null && directory.IsDirectory) {
//            foreach (var file in directory.ListFiles()) {
//                if (file.IsDirectory) {
//                    folderUris.Add(file.Uri);
//                }
//                else {
//                    bool isValid = false;
//                    foreach (var extension in Setting.SupportedExtension) {
//                        if (file.Name.Contains(extension)) {
//                            isValid = true;
//                            break;
//                        }
//                    }
//                    if (isValid) {
//                        fileUris.Add(file.Uri);
//                    }
//                }
//            }
//        }
//        return (folderUris, fileUris);
//    }
//    public static string DirectFilePath(string filePath) {
//        string[] parts = filePath.Split("primary:");
//        string result = "/storage/emulated/0/" + parts[^1];
//        return result;
//    }
//    public static string GetDirectPathFromUri(Uri uri) {
//        string folderPath = uri.EncodedPath;
//        string decodedPath = UriUtil.Decode(folderPath);
//        string directPath = UriUtil.DirectFilePath(decodedPath);
//        return directPath;
//    }
//    public static string GetPathFromUri(Uri uri) {
//        string docId = DocumentsContract.GetTreeDocumentId(uri);
//        string[] split = docId.Split(":");
//        string type = split[0];
//        string relativePath = split.Length > 1 ? split[1] : "";
//        string fullPath = "";
//        if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase)) {
//            fullPath = Enviroment.ExternalStorageDirectory + "/" + relativePath;
//        }
//        else {
//            var externalDirs = Application.Context.GetExternalFilesDirs(null);
//            if (externalDirs != null && externalDirs.Length > 1) {
//                foreach (var dir in externalDirs) {
//                    if (dir.AbsolutePath.Contains(type)) {
//                        fullPath = dir.AbsolutePath.Substring(0, dir.AbsolutePath.IndexOf("Android")) + relativePath;
//                        break;
//                    }
//                }
//            }
//        }
//        return fullPath;
//    }
//    public static async Task RequestSinglePermission<T>() where T: Permissions.BasePermission {
//        var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
//        if (status != PermissionStatus.Granted) {
//            System.Diagnostics.Debug.WriteLine("Permission denied");
//        }
//        else {
//            System.Diagnostics.Debug.WriteLine("Permission granted");
//        }
//    }
      
//    public static async Task RequestPermission() {
//        await RequestSinglePermission<Permissions.StorageWrite>();
//    }
//    public static Uri? GetUriFomPath(string filePath) {
//        var file = new Java.IO.File(filePath);
//        var contentUri = Uri.FromFile(file);
//        return contentUri;
//    }
//    public static List<string> GetFiles(Uri directoryUri) {
//        Activity activity = Platform.CurrentActivity;
//        List<string> files = new List<string>();
//        var contentResolver = activity.ContentResolver;
//        var cursor = contentResolver.Query(directoryUri, null, null, null, null);
//        if (cursor != null) {
//            while (cursor.MoveToNext()) {
//                string fileUri = cursor.GetString(cursor.GetColumnIndex(OpenableColumns.DisplayName));
//                files.Add(fileUri);
//            }
//            cursor.Close();
//        }
//        return files;
//    }
//}
//#pragma warning restore CS8600, CS8604, CS8603, CS8602