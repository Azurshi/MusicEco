using Android;
using Uri = Android.Net.Uri;

using AndroidF = Android;
using Android.Provider;
using Microsoft.Win32.SafeHandles;
using Android.OS;

namespace MusicEco.Core.Platforms.Android;


public static class UriUtility {
    private const string FolderMimeType = "vnd.android.document/directory";
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
    public static async Task<bool> RequestSinglePermission<T>() where T : Permissions.BasePermission {
        var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted) {
            return false;
        }
        else {
            return true;
        }
    }

    public static async Task<bool> RequestStoragePermission() {
        return await RequestSinglePermission<Permissions.StorageWrite>();
    }
    public static List<Uri> GetFiles(Uri folder) {
        return GetItems(folder, true);
    }
    public static List<Uri> GetFolder(Uri folder) {
        return GetItems(folder, false);
    }
    private static List<Uri> GetItems(Uri treeUri, bool fileOnly) {
        List<Uri> items = [];
        var context = AndroidF.App.Application.Context;
        var resolver = context.ContentResolver!;
        string documentId = DocumentsContract.GetTreeDocumentId(treeUri)!;
        var childrenUri = DocumentsContract.BuildChildDocumentsUriUsingTree(treeUri, documentId)!;
        string[] projection = [
            DocumentsContract.Document.ColumnDocumentId,
            DocumentsContract.Document.ColumnMimeType
            ];
        using (var cursor = resolver.Query(
            childrenUri,
            projection,
            null,
            null,
            null)) {
            if (cursor == null) {
                return items;
            }
            int idIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnDocumentId);
            int mimeIndex = cursor.GetColumnIndex(DocumentsContract.Document.ColumnMimeType);
            while (cursor.MoveToNext()) {
                string childDocumentId = cursor.GetString(idIndex)!;
                string mimeType = cursor.GetString(mimeIndex)!;
                bool isFolder = mimeType == FolderMimeType;
                if (isFolder && fileOnly) {
                    continue;
                }
                var childUri = DocumentsContract.BuildDocumentUriUsingTree(treeUri, childDocumentId);
                items.Add(childUri!);
            }
        }
        return items;
    }
    public static FileStream OpenFile(Uri uri, int bufferSize, FileAccess fileAccess) {
        var context = AndroidF.App.Application.Context;
        var resolver = context.ContentResolver!;
        SafeFileHandle? handle = null;
        string flag = fileAccess switch {
            FileAccess.Read => "r",
            FileAccess.Write => "w",
            FileAccess.ReadWrite => "rw",
            _ => throw new ArgumentException(null, nameof(fileAccess))
        };
        using (var parcelFileDesriptor = resolver.OpenFileDescriptor(uri, flag) ?? throw new InvalidOperationException($"Failed to open: {uri.Path}")) {
            int fd = parcelFileDesriptor.DetachFd();
            try {
                handle = new SafeFileHandle(fd, ownsHandle: true);
                return new FileStream(handle, fileAccess, bufferSize, false);
            }
            catch {
                if (handle != null) {
                    handle.Close();
                }
                else {
                    using (var cleanup = ParcelFileDescriptor.AdoptFd(fd)) {

                    }
                }
                throw;
            }
        }
    }
}
